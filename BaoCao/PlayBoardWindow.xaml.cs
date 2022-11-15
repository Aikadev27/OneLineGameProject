using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
 * [x]xóa nút ==> chọn nút + del
 * [x] xóa cung
 * [x] vẽ cung ===> (shift + click trên nút) (di chuột + đè shift + nút chưa được thả)
 * [x] hủy cung ===> vẽ cung --> thả chuột không trên nút nào
 * [x] xóa cung, xóa node ---> xóa nó ra khỏi list
 * 
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
            private set
            {
                _selectedNode?.Default();
                _selectedNode?.ClearFocus();
                _selectedNode = value;
                _selectedNode?.Select();
                _selectedNode?.SetFocus();
            }
        }
        public Edge SelectedEdge
        {
            get => _selectedEdge;
            private set => _selectedEdge = value;
        }


        public bool IsLeftCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl);
        public bool IsMouseLeftButtonDown => Mouse.LeftButton == MouseButtonState.Pressed;
        public bool IsLeftAltDown => Keyboard.IsKeyDown(Key.LeftAlt);
        public bool IsLeftShiftDown => Keyboard.IsKeyDown(Key.LeftShift);
        public bool IsDelKeyDown => Keyboard.IsKeyDown(Key.Delete);

        #endregion



        public PlayBoardWindow()
        {
            InitializeComponent();

            _canvasGameBoard.Background = Constants.BoardBackgroundColor;

            _nodeList = new List<Node>();
            _edgeList = new List<Edge>();
            SelectedNode = null;
            SelectedEdge = null;

            _canvasGameBoard.MouseDown += (sender, e) =>
            {
                if (IsLeftCtrlDown && IsMouseLeftButtonDown)
                {
                    var pos = e.GetPosition(_canvasGameBoard);
                    var node = CreateNode(pos.X, pos.Y);
                    _nodeList.Add(node);
                }
            };
            _canvasGameBoard.MouseMove += (sender, e) =>
            {
                var pos = e.GetPosition(_canvasGameBoard);
                if (SelectedNode != null && IsLeftAltDown
                    && SelectedNode.CheckPointIn(pos)
                    && IsMouseLeftButtonDown)
                {
                    SelectedNode.SetCenterLocation(pos);
                }

                // ve cung :))
                if (SelectedEdge != null && IsLeftShiftDown && IsMouseLeftButtonDown)
                {
                    SelectedEdge.SetEndPoint(pos);
                }
                else if (SelectedEdge != null)
                {
                    bool IsAccept = false;
                    foreach (var node in _nodeList)
                    {
                        if (node.CheckPointIn(pos) && node != SelectedEdge.UNode)
                        {
                            // accept edge 
                            SelectedEdge.SetVNode(node);
                            SelectedEdge.EdgeRemove += (sender1, e1) => _edgeList.Remove(sender1 as Edge);
                            _edgeList.Add(SelectedEdge);

                            SelectedEdge = null;
                            IsAccept = true;
                            break;
                        }
                    }
                    // huy cung
                    if (!IsAccept)
                    {
                        SelectedEdge.RemoveParent();
                        SelectedEdge = null;
                    }
                }
            };
        }

        public Node CreateNode(double x, double y)
        {
            Node node = new Node();
            node.SetParent(_canvasGameBoard);
            node.SetCenterLocation(new Point(x, y));
            // TODO: tach node.click ra ham rieng
            node.Click += (sender1, e1) =>
            {
                SelectedNode = node;
                e1.Handled = true;
                if (IsLeftShiftDown)
                {
                    Edge edge = new Edge(_canvasGameBoard, SelectedNode);
                    edge.SetEndPoint(SelectedNode.GetCenterLocation()); // !IMPORTANT : khong duoc xoa dong nay

                    SelectedEdge = edge;
                }
            };
            node.NodeRemove += (sender, e) =>
            {
                Stack<int> edgeRemoveIndex = new Stack<int>();
                for (int i = 0; i < _edgeList.Count; i++)
                {
                    Edge edge = _edgeList[i];
                    if (edge.UNode == node || edge.VNode == node)
                    {
                        edgeRemoveIndex.Push(i);
                    }
                }
                while (edgeRemoveIndex.Count > 0)
                {
                    _edgeList[edgeRemoveIndex.Pop()].RemoveParent();
                }
                _nodeList.Remove(node);
            };

            return node;
        }
    }
}
