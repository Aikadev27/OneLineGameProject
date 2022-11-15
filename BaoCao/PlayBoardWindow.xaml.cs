using Microsoft.Win32;
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
 * [x] xuất dữ liệu ra file
 * [x] đọc dữ liệu từ file
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



        public PlayBoardWindow(bool isDesignMode=false)
        {
            InitializeComponent();

            // TODO: chinh sua giao dien, them nut export vao giao dien
            Button btn = new Button() { Content = "export" };
            btn.Click += (sender, e) =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    Tool.ExportGraphToFile(filePath, _nodeList, _edgeList);
                }
            };

            // TODO: chinh sua giao dien, them nut read vao giao dien
            Button btnRead = new Button() { Content = "read" };
            btnRead.Click += (sender, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    #region xoa du lieu node va edge cu
                    for (int i = _edgeList.Count - 1; i >= 0; i--)
                    {
                        _edgeList[i].RemoveParent();
                    }
                    _edgeList.Clear();
                    for (int i = _nodeList.Count - 1; i >= 0; i--)
                    {
                        _nodeList[i].RemoveParent();
                    }
                    _nodeList.Clear();
                    #endregion


                    List<Node> tempNodes;
                    List<Edge> tempEdges;
                    Tool.ReadGraphFromFile(filePath, out tempNodes, out tempEdges, CreateNode, CreateEdge);
                    _edgeList = tempEdges;
                    _nodeList = tempNodes;
                }
            };
            _toolBar.Items.Add(btn);
            _toolBar.Items.Add(btnRead);


            _canvasGameBoard.Background = Constants.BoardBackgroundColor;

            _nodeList = new List<Node>();
            _edgeList = new List<Edge>();
            SelectedNode = null;
            SelectedEdge = null;

            _canvasGameBoard.MouseDown += CanvasGameBoard_MouseDownEvent;
            _canvasGameBoard.MouseMove += CanvasGameBoard_MouseMove;
        }

        private void CanvasGameBoard_MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (IsLeftCtrlDown && IsMouseLeftButtonDown)
            {
                var pos = e.GetPosition(_canvasGameBoard);
                var node = CreateNode(pos.X, pos.Y);
                _nodeList.Add(node);
            }
        }

        private void CanvasGameBoard_MouseMove(object sender, MouseEventArgs e)
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
                        Node uNode = SelectedEdge.UNode;
                        Node vNode = node;

                        SelectedEdge.RemoveParent();
                        SelectedEdge = CreateEdge(uNode, vNode);
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
        }

        private void Node_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            SelectedNode = sender as Node;
            e.Handled = true;
            if (IsLeftShiftDown)
            {
                Edge edge = new Edge(_canvasGameBoard, SelectedNode);
                edge.SetEndPoint(SelectedNode.GetCenterLocation()); // !IMPORTANT : khong duoc xoa dong nay
                SelectedEdge = edge;
            }
        }

        public Node CreateNode(double x, double y)
        {
            Node node = new Node();
            node.SetParent(_canvasGameBoard);
            node.SetCenterLocation(new Point(x, y));
            // TODO: tach node.click ra ham rieng
            node.Click += Node_ClickEvent;
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
            Debug.WriteLine($"tao node: {x} {y}");
            return node;
        }

        public Edge CreateEdge(Node u, Node v)
        {
            Edge edge = new Edge(_canvasGameBoard, u, v);
            edge.EdgeRemove += (sender1, e1) => _edgeList.Remove(sender1 as Edge);
            Debug.WriteLine($"tao edge: ({u.GetCenterLocation()},{u.NodeText})   ({v.GetCenterLocation()},{v.NodeText})");
            return edge;
        }
    }
}
