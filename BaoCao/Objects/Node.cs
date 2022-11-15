using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;

namespace BaoCao
{
    public class Node : INotifyPropertyChanged
    {
        private TextBlock _textBlock;
        private Ellipse _elp;
        private Grid _grid;
        private Canvas _parent;
        private TextBox _textBoxHidden;


        public object Tag { get; set; }
        public double NodeWidth => _elp.Width;
        public double NodeHeight => _elp.Height;
        public string NodeText { get => _textBlock.Text; set => _textBoxHidden.Text = value; }


        #region node property changed event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion

        #region node click event
        private event EventHandler<MouseButtonEventArgs> _click;
        public event EventHandler<MouseButtonEventArgs> Click { add => _click += value; remove => _click -= value; }
        private void OnClick(MouseButtonEventArgs e) => _click?.Invoke(this, e);
        #endregion

        #region node remove (remove parent) event
        private event EventHandler _nodeRemove;
        public event EventHandler NodeRemove { add => _nodeRemove += value; remove => _nodeRemove -= value; }
        private void OnNodeRemove() => _nodeRemove?.Invoke(this, new EventArgs());
        #endregion

        public Node()
        {
            _parent = null;
            _grid = new Grid();

            _textBlock = new TextBlock()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = Constants.NodeFontSize
            };
            _elp = new Ellipse()
            {
                Width = Constants.NodeSize,
                Height = Constants.NodeSize,
                Fill = Brushes.OrangeRed
            };
            _textBoxHidden = new TextBox()
            {
                Width = 0,
                Height = 0
            };

            Button btnRemove = new Button() { Content = "remove" };
            TextBox txb = new TextBox()
            {
                MinWidth = 50
            };
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(txb);
            contextMenu.Items.Add(btnRemove);

            btnRemove.Click += (sender, e) =>
            {
                this.RemoveParent();
                contextMenu.IsOpen = false;
            };


            _grid.Children.Add(_elp);
            _grid.Children.Add(_textBlock);
            _grid.Children.Add(_textBoxHidden);
            _elp.ContextMenu = contextMenu;

            _textBlock.SetBinding(TextBlock.TextProperty,
                new System.Windows.Data.Binding(nameof(_textBoxHidden.Text)) { Source = _textBoxHidden });

            txb.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding(nameof(_textBoxHidden.Text))
            {
                Source = _textBoxHidden,
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            });

            _elp.MouseDown += (sender, e) => OnClick(e);
        }

        public void SetParent(Canvas parent)
        {
            _parent = parent;
            _parent.Children.Add(_grid);
        }

        public void RemoveParent()
        {
            _parent.Children.Remove(_grid);
            _parent = null;
            OnNodeRemove(); // invoke NodeRemove event
        }

        public void SetLocation(Point point)
        {
            Canvas.SetLeft(_grid, point.X);
            Canvas.SetTop(_grid, point.Y);
            OnPropertyChanged("Location");
        }

        public void SetCenterLocation(Point centerPoint)
        {
            centerPoint.X -= NodeWidth / 2;
            centerPoint.Y -= NodeHeight / 2;
            SetLocation(centerPoint);
        }

        public Point GetLocation()
        {
            return new Point(Canvas.GetLeft(_grid), Canvas.GetTop(_grid));
        }

        public Point GetCenterLocation()
        {
            return new Point(Canvas.GetLeft(_grid) + NodeWidth / 2, Canvas.GetTop(_grid) + NodeHeight / 2);
        }

        public bool CheckPointIn(Point point)
        {
            var topLeft = GetLocation();
            point.X -= topLeft.X;
            point.Y -= topLeft.Y;
            return 0 <= point.X && point.X <= NodeWidth
                && 0 <= point.Y && point.Y <= NodeHeight;
        }

        public void Default()
        {
            _elp.Fill = Constants.NodeBackgroundColorDefault;
        }

        public void Select()
        {
            _elp.Fill = Constants.NodeBackgroundColorSelect;
        }

        public void SetFocus()
        {
            _textBoxHidden.Focus();
        }

        public void ClearFocus()
        {
            Keyboard.ClearFocus();
        }
    }
}
