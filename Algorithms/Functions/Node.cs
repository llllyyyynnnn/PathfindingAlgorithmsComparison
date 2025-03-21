using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingAlgorithmsComparison.Algorithms
{
    /// <summary>
    ///  A node in the graph.
    /// </summary>
    public class Node
    {
        /// <summary>
        ///  The ID of the node.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        ///  The name of the node. Is created automaticly.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        ///  A collection of all edges starting at this node.
        /// </summary>
        public List<Edge> Edges { get; private set; }

        /// <summary>
        ///  Create a new node.
        /// </summary>
        /// <param name="id">The ID of the new node. Should be from 0 to maximum in the graph - 1.</param>
        /// <exception cref="ArgumentException">the ID was bad.</exception>
        internal Node(int id)
        {
            if (id < 0) {
                throw new ArgumentException("Bad node id!");
            }
            Id = id;
            Name = "Node " + id;
            Edges = new List<Edge>();
        }

        /// <summary>
        ///  Output a string showing the node.
        /// </summary>
        /// <returns>the output string.</returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append($"Node: {Id}:{Name} Neighbors:\n");
            foreach (Edge e in Edges) {
                output.Append("    " + e.ToString() + "\n");
            }
            return output.ToString();
        }

        private void AddEdge(int end, int length)
        {
            if (end < 0) {
                throw new ArgumentException("Bad end edge!");
            }
            Edges.Add(new Edge(Id, end, length));
        }
    }
}
