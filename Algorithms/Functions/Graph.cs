using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingAlgorithmsComparison.Algorithms
{
    /// <summary>
    ///  A graph.
    /// </summary>
    public class Graph
    {
        /// <summary>
        ///  The maximum edge length in the graph.
        /// </summary>
        public const double MAX_EDGE_LENGTH = 1000.0;
        /// <summary>
        ///  The nodes of this graph.
        /// </summary>
        public List<Node> Nodes { get; private set; }
        /// <summary>
        ///  The edges of this graph.
        /// </summary>
        public double[,]  Edges { get; private set; }

        /// <summary>
        ///  Creates a new graph.
        /// </summary>
        /// <param name="nodes">the number of nodes.</param>
        /// <param name="percentEdges">the percentage of edges for each node. Do not set this to above 80%.</param>
        /// <param name="doubleSidedEdges">should the edges be double-sided?</param>
        public Graph(int nodes, int percentEdges, bool doubleSidedEdges = false)
        {
            Nodes = new List<Node>();
            for (int i = 0; i < nodes; i++) {
                Nodes.Add(new Node(i));
            }
            CreateEdges(percentEdges, doubleSidedEdges);
        }

        /// <summary>
        ///  Print the length of all edges of this graph as a node x node table.
        /// </summary>
        public void Print()
        {
            Print(Edges);
        }

        /// <summary>
        ///  Print a distance table of this graph as a node x node table.
        /// </summary>
        /// <param name="distances">the distance table.</param>
        /// <exception cref="ArgumentException">something wasn't right.</exception>
        public void Print(double[,] distances)
        {
            if (Nodes.Count != distances.GetLength(0) || Nodes.Count != distances.GetLength(1)) {
                throw new ArgumentException("The distances argument has the wrong dimensions.");
            }
            foreach(Node n in Nodes) {
                Console.Write("\t{0}", n.Id);
            }
            Console.WriteLine();
            foreach(Node n in Nodes) {
                Console.Write("{0}", n.Id);
                foreach(Node d in Nodes) {
                    if (distances[n.Id, d.Id] < int.MaxValue) {
                        Console.Write("\t({0})", (int)distances[n.Id, d.Id]);
                    } else {
                        Console.Write("\tInf");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        ///  Output a string showing the graph.
        /// </summary>
        /// <returns>the output string.</returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append($"Graph nodes: {Nodes.Count}\n");
            foreach (Node n in Nodes) {
                output.Append("  " + n.ToString());
            }
            return output.ToString();
        }

        private void CreateEdges(int percentEdges, bool doubleSidedEdges)
        {
            if (percentEdges < 0 || 100 <= percentEdges) {
                throw new ArgumentException("Bad percent of edges.");
            }
            Edges = new double[Nodes.Count, Nodes.Count];
            foreach (Node nodeA in Nodes) {
                foreach (Node nodeB in Nodes) {
                    Edges[nodeA.Id, nodeB.Id] = int.MaxValue;
                }
                Edges[nodeA.Id, nodeA.Id] = 0;
            }
            int edgesLeft = (int)(0.01 * percentEdges * Nodes.Count * Nodes.Count);
            double length = 0;
            int start = 0;
            int end = 0;

            while (edgesLeft > 0) {
                start = random.Next(0, Nodes.Count);
                end = random.Next(0, Nodes.Count);
                length = MAX_EDGE_LENGTH * random.NextDouble();
                length = (length == 0.0 ? 1.0 : length);

                // Using double sided edges.
                if (doubleSidedEdges) {
                    if (start != end && Edges[start, end] >= int.MaxValue && Edges[end, start] >= int.MaxValue) {
                        edgesLeft--;
                        Edges[start, end] = length;
                        Nodes[start].Edges.Add(new Edge(start, end, length));
                        Edges[end, start] = length;
                        Nodes[end].Edges.Add(new Edge(end, start, length));
                    }
                } else {
                    if (start != end && Edges[start, end] >= int.MaxValue) {
                        edgesLeft--;
                        Edges[start, end] = length;
                        Nodes[start].Edges.Add(new Edge(start, end, length));
                    }
                }
            }
        }

        Random random = new Random();
    }
}
