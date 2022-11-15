using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BaoCao
{
    public static class Tool
    {
        public static void ExportGraphToFile(string path, List<Node> nodes, List<Edge> edges)
        {
            #region danh index cho tung node trong nodes, va cho tung edge trong edges
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Tag = i;
            }
            for (int i = 0; i < edges.Count; i++)
            {
                edges[i].Tag = i;
            }
            #endregion

            #region xay dung chuoi du lieu de xuat ra file
            StringBuilder datalines = new StringBuilder();

            // first line: n dinh|m cung
            datalines.AppendLine($"{nodes.Count}|{edges.Count}");

            // n next lines: x|y|text
            foreach (Node node in nodes)
            {
                var pos = node.GetCenterLocation();
                datalines.AppendLine($"{pos.X}|{pos.Y}|{node.NodeText}");
            }

            // m next lines: uNodeIndex|vNodeIndex
            foreach (var edge in edges)
            {
                datalines.AppendLine($"{edge.UNode.Tag}|{edge.VNode.Tag}");
            }
            #endregion

            File.WriteAllText(path, datalines.ToString());
        }

        public static bool ReadGraphFromFile(string path, out List<Node> nodes, out List<Edge> edges, 
            Func<double, double, Node> createNodeFunc,
            Func<Node, Node, Edge> createEdgeFunc)
        {
            nodes = new List<Node>();
            edges = new List<Edge>();

            var dataLines = File.ReadAllLines(path);
            // n|m
            string[] args = dataLines[0].Split('|');

            int nodeSize, edgeSize;
            if (!Int32.TryParse(args[0], out nodeSize) 
                || !Int32.TryParse(args[1], out edgeSize)) return false;

            double x, y;
            for (int i = 1; i <= nodeSize; i++)
            {
                // x|y|text
                args = dataLines[i].Split('|');

                if (!double.TryParse(args[0], out x) 
                    || !double.TryParse(args[1], out y)) return false;

                Node node = createNodeFunc(x, y);
                node.NodeText = args[2];
                node.Tag = i - 1;
                nodes.Add(node);
            }

            int uIndex, vIndex;
            for (int i = nodeSize + 1; i <= nodeSize + edgeSize; i++)
            {
                // uNodeIndex|vNodeIndex
                args = dataLines[i].Split('|');
                if (!Int32.TryParse(args[0], out uIndex) 
                    || !Int32.TryParse(args[1], out vIndex)) return false;
                Edge edge = createEdgeFunc(nodes[uIndex], nodes[vIndex]);
                edge.Tag = i - nodeSize - 1;
                edges.Add(edge);
            }

            return true;
        }
    }
}
