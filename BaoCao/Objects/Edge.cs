using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BaoCao
{
    public class Edge
    {
        private Line _line;
        private Canvas _parent;
        private Node _uNode;
        private Node _vNode;

        public Node UNode { get => _uNode; private set => _uNode = value; }
        public Node VNode { get => _vNode; private set => _vNode = value; }

        public Edge()
        {
            //< Line X1 = "20" X2 = "150" Y1 = "13" Y2 = "200"
            //      Stroke = "Black" StrokeThickness = "2" />
            _parent = null;
            UNode = null;
            VNode = null;
            _line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetZIndex(_line, -1);
        }

        private void SetStartPoint(Point startPoint)
        {
            _line.X1 = startPoint.X;
            _line.Y1 = startPoint.Y;
        }

        public void SetEndPoint(Point endPoint)
        {
            _line.X2 = endPoint.X;
            _line.Y2 = endPoint.Y;
        }

        public void SetUNode(Node uNode)
        {
            UNode = uNode;
            SetStartPoint(uNode.GetCenterLocation());


            UNode.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                {
                    SetStartPoint(UNode.GetCenterLocation());
                }
            };
        }

        public void SetVNode(Node vNode)
        {
            VNode = vNode;
            SetEndPoint(vNode.GetCenterLocation());

            VNode.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                {
                    SetEndPoint(VNode.GetCenterLocation());
                }
            };
        }

        public void AddParent(Canvas canvas)
        {
            _parent = canvas;
            _parent.Children.Add(_line);
        }

        public void RemoveParent()
        {
            if (_parent != null)
            {
                _parent.Children.Remove(_line);
                _parent = null;
            }
        }
    }
}
