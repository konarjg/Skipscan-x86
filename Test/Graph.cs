using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Node
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public int Group { get; set; }

        public Node(string key, int group, string name)
        {
            Key = key;
            Group = group;
            Name = name;
        }
    }

    public class Edge
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public int Weight { get; set; }

        private int DetermineWeight()
        {
            if (Left == null || Right == null)
                return -1;

            var leftGroup = Left.Group;
            var rightGroup = Right.Group;

            switch (leftGroup)
            {
                case int group when group == rightGroup:
                    return 1;

                case int group when Math.Abs(group - rightGroup) == 1:
                    return 5;
            }

            return -1;
        }

        public Edge(Node left, Node right)
        {
            Left = left;
            Right = right;
            Weight = DetermineWeight();

            if (Weight == -1)
                throw new ArgumentException("Illegal Edge Exception");
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0} -> {1} {2}", Left.Name, Right.Name, Weight);

            return builder.ToString();
        }
    }

    public class Graph
    {
        public List<Node> Nodes { get; set; }
        public Queue<Edge> Edges { get; set; }

        public Graph(List<Node> nodes)
        {
            Nodes = nodes;
            Edges = new Queue<Edge>();

            foreach (var left in Nodes)
            {
                foreach (var right in Nodes)
                {
                    try
                    {
                        if (left.Name == right.Name)
                            continue;

                        if (Edges.Where(edge => edge.Left.Name == left.Name && edge.Right.Name == right.Name).Count() != 0)
                            continue;

                        if (Edges.Where(edge => edge.Left.Name == right.Name && edge.Right.Name == left.Name).Count() != 0)
                            continue;

                        Edges.Enqueue(new Edge(left, right));
                    }
                    catch (ArgumentException e)
                    {
                        continue;
                    }
                }
            }
        }

        public void Sort()
        {
            var copy = new List<Edge>();

            foreach (var edge in Edges)
                copy.Add(edge);

            Edges.Clear();
            var sorted = copy.OrderBy(edge => edge.Weight);

            foreach (var edge in sorted)
                Edges.Enqueue(edge);
        }

        public List<Node> MinimalProgram()
        {
            var result = new List<Node>();
            var copy = new Queue<21>

        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var edge in Edges)
                builder.AppendLine(edge.ToString());

            return builder.ToString();
        }
    }
}
