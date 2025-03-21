using System;

namespace PathfindingAlgorithmsComparison.Algorithms
{
    /// <summary>
    ///  An edge in the graph.
    /// </summary>
    public class Edge
    {
        /// <summary>
        ///  The start node ID.
        /// </summary>
        public int Start { get; private set; }
        /// <summary>
        ///  The end node ID.
        /// </summary>
        public int End { get; private set; }
        /// <summary>
        ///  The length of the edge.
        /// </summary>
        public double Length { get; private set; }

        /// <summary>
        ///  Create a new edge.
        /// </summary>
        /// <param name="start">the node ID of the start node.</param>
        /// <param name="end">the node ID of the end node.</param>
        /// <param name="length">the length of this graph.</param>
        /// <exception cref="ArgumentException">something went bad.</exception>
        internal Edge(int start, int end, double length)
        {
            if (start < 0 || end < 0 || length <= 0) {
                throw new ArgumentException("Bad edge!");
            }
            Start = start;
            End = end;
            Length = length;
        }

        /// <summary>
        ///  Output a string showing the edge.
        /// </summary>
        /// <returns>the output string.</returns>
        public override string ToString()
        {
            return $"Start {Start}---{Length}--->End {End}";
        }
    }
}
