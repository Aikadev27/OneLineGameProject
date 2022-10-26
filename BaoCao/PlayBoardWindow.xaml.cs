using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;


/**
 * [x] chọn nút ===> click nút
 * [x] vẽ nút ===> ctrl + click chuột trên canvas
 * [x] di chuyển nút ===> alt + click chuột trên nút + di chuột
 * [x] xóa nút ==> chọn nút + del
 * [ ] vẽ cung ===> (shift + click trên nút) (di chuột + đè shift + nút chưa được thả)
 * [ ] hủy cung ===> vẽ cung --> thả chuột không trên nút nào
 *  
 * [ ] xóa cung, xóa node ---> xóa nó ra khỏi list
 */

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for PlayBoardWindow.xaml
    /// </summary>
    public partial class PlayBoardWindow : Window
    {
        private Node _selectedNode;
        private Edge _selectedEdge;
        private List<Node> _nodeList;
        private List<Edge> _edgeList;


        #region properties
        public Node SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode?.Default();
                _selectedNode = value;
                _selectedNode?.Select();
            }
        }

        public bool IsLeftCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl);
        public bool IsMouseLeftButtonDown => Mouse.LeftButton == MouseButtonState.Pressed;
        public bool IsLeftAltDown => Keyboard.IsKeyDown(Key.LeftAlt);
        public bool IsLeftShiftDown => Keyboard.IsKeyDown(Key.LeftShift);
        #endregion



        public PlayBoardWindow()
        {
            InitializeComponent();

            _canvasGameBoard.Background = Constants.BoardBackgroundColor;

            _nodeList = new List<Node>();
            _edgeList = new List<Edge>();
            SelectedNode = null;
            _selectedEdge = null;

            Button btn = new Button()
            {
                Content = "",
                Width = 200,
                Height = 50,
                FontSize = 24
            };

            //Edge edge = new Edge();
            //edge.AddParent(_canvasGameBoard);
            //edge.SetStartPoint(new Point(0, 0));
            //edge.SetEndPoint(new Point(20, 13));

            _canvasGameBoard.MouseDown += (sender, e) =>
            {

                if (IsLeftCtrlDown)
                {
                    var mousePosition = e.GetPosition(_canvasGameBoard);

                    Node node = new Node();
                    node.AddParent(_canvasGameBoard);
                    node.SetCenterLocation(mousePosition);
                    node.Click += (sender1, e1) =>
                    {
                        SelectedNode = node;
                        if (IsLeftShiftDown)
                        {
                            Edge edge = new Edge();
                            edge.AddParent(_canvasGameBoard);
                            edge.SetUNode(SelectedNode);
                            edge.SetEndPoint(SelectedNode.GetCenterLocation());
                            _selectedEdge = edge;
                        }
                    };
                    _nodeList.Add(node);
                }
                else
                {
                    SelectedNode = null;
                    _selectedEdge = null;
                }
            };

            _canvasGameBoard.MouseMove += (sender, e) =>
            {

                if (IsLeftAltDown && SelectedNode != null && IsMouseLeftButtonDown)
                {
                    SelectedNode.SetCenterLocation(e.GetPosition(_canvasGameBoard));
                }
                else if (_selectedEdge != null )
                {
                    if (IsMouseLeftButtonDown && IsLeftShiftDown)
                    {
                        _selectedEdge.SetEndPoint(e.GetPosition(_canvasGameBoard));
                    }
                    else
                    {
                        var mousePosition = e.GetPosition(_canvasGameBoard);

                        foreach (var node in _nodeList)
                        {
                            if (node.CheckPointIn(mousePosition))
                            {
                                _selectedEdge.SetVNode(node);
                                _edgeList.Add(_selectedEdge);
                                _selectedEdge = null;
                                return;
                            }
                        }                        
                                                
                        _selectedEdge.RemoveParent();
                        _selectedEdge = null;
                        
                    }
                }
            };

            this.KeyDown += (sender, e) =>
            {
                if (Keyboard.IsKeyDown(Key.Delete))
                {
                    _selectedNode?.RemoveParent();
                    _selectedNode = null;
                }
            };
        }

    }
}
