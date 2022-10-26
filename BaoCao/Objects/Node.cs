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
        private Grid _grid;
        private Ellipse _ellipse;
        private TextBlock _textBlock;
        private Canvas _parent;

        

        public Node()
        {
            //< Grid  >
            //    < Ellipse Width = "40" Height = "40" Fill = "DarkOrange"/>
            //    < TextBox MinWidth = "12" Background = "Transparent" VerticalAlignment = "Center" HorizontalAlignment = "Center" BorderBrush = "Transparent" />
            //</ Grid >             
            _parent = null;
            _grid = new Grid();
            _ellipse = new Ellipse()
            {
                Width = 40,
                Height = 40,
                Fill = Constants.NodeBackgroundColorDefault,                
            };
            _textBlock = new TextBlock()
            {
                MinWidth = 12,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 16,
            };


            ContextMenu menu = new ContextMenu();
            TextBox txb = new TextBox() { MinWidth = 40 };

            //_textBlock.Text <= txb.Text;
            _textBlock.SetBinding(
                TextBlock.TextProperty,
                new System.Windows.Data.Binding("Text")
                {
                    Source = txb
                });
            Button btn = new Button()
            {
                Content = "remove"
            };
            btn.Click += (sender, e) =>
            {
                this.RemoveParent();
                menu.IsOpen = false;
            };

            menu.Items.Add(txb);
            menu.Items.Add(btn);
            _grid.ContextMenu = menu;


            _grid.Children.Add(_ellipse);
            _grid.Children.Add(_textBlock);


            _grid.MouseDown += (sender, e) =>
            {
                e.Handled = true;
                OnClick();
            };           
        }

        
        public Point GetLocation()
        {
            Point locate = new Point(
                    x: Canvas.GetLeft(_grid), 
                    y: Canvas.GetTop(_grid));            
            return locate;
        }

        public Point GetCenterLocation()
        {
            Point locate = new Point(
                    x: Canvas.GetLeft(_grid) + _ellipse.Width / 2,
                    y: Canvas.GetTop(_grid) + _ellipse.Height / 2);
            return locate;
        }

        public void AddParent(Canvas canvas)
        {
            _parent = canvas;
            _parent.Children.Add(_grid);
        }

        public void RemoveParent()
        {
            if (_parent != null)
            {
                _parent.Children.Remove(_grid);
                _parent = null;
            }
        }

        public void SetLocation(Point locate)
        {
            Canvas.SetLeft(_grid, locate.X);
            Canvas.SetTop(_grid, locate.Y);

            OnPropertyChanged("Location");
        }

        public void SetCenterLocation(Point topLeftLocate)
        {
            topLeftLocate.X -= _ellipse.Width / 2;
            topLeftLocate.Y -= _ellipse.Height / 2;

            Canvas.SetLeft(_grid, topLeftLocate.X);
            Canvas.SetTop(_grid, topLeftLocate.Y);

            OnPropertyChanged("Location");
        }

        public bool CheckPointIn(Point locate)
        {
            var topLeft = GetLocation();
            var bottomRight = new Point(topLeft.X + _ellipse.Width, topLeft.Y + _ellipse.Height);
            return topLeft.X <= locate.X && locate.X <= bottomRight.X
                && topLeft.Y <= locate.Y && locate.Y <= bottomRight.Y;
        }

        public void Select()
        {
            _ellipse.Fill = Constants.NodeBackgroundColorSelect;
            _ellipse.StrokeThickness = 2;
            _ellipse.Stroke = Brushes.Coral;
        }

        public void Default()
        {
            _ellipse.Fill = Constants.NodeBackgroundColorDefault;
            _ellipse.StrokeThickness = 0;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private event EventHandler _click;
        public event EventHandler Click
        {
            add => _click += value;
            remove => _click -= value;
        }
        private void OnClick()
        {
            _click?.Invoke(this, new EventArgs());
        }
    }
}
