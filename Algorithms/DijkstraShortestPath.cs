namespace PathfindingAlgorithmsComparison.Algorithms;

public class DijkstraShortestPath
{
    public static double[,] Execute(Graph graph)
    {
        int n = graph.Nodes.Count; // nodeCount
        double[,] distances = new double[n, n]; // like we did in FloydWarshalls algorithm, we copy the nodes so we don't make any changes to the original ones

        for (int i = 0; i < n; i++)
        {
            PriorityQueue<int, double> priorityQueue = new PriorityQueue<int, double>(); // create a priorityQueue instance, which is great for tasks like this since we have to prioritize shortest paths, etc.
            double[] minDistances = new double[n]; // we store the shortest distances here
            Array.Fill(minDistances, double.MaxValue);
            minDistances[i] = 0;

            bool[] visited = new bool[n]; // we store the nodes we already visited here
            priorityQueue.Enqueue(i, 0); // since it's our starting node, the distance has to be 0

            while (priorityQueue.TryDequeue(out int index, out double distance)) // loop through all nodes
            {
                if (visited[index]) // ignore if we already visited this index (node)
                    continue;

                visited[index] = true;

                if (distance > minDistances[index])
                    continue;

                foreach (Edge edge in graph.Nodes[index].Edges)  // for every edge in the nodes, we check if their distance alongside our current one results in a shorter or a longer path
                {
                    if (visited[edge.End]) // don't revisit old nodes
                        continue;

                    double newDistance = distance + edge.Length;
                    if (newDistance < minDistances[edge.End]) // if the new distance is outweighed by the old one, we update it
                    {
                        minDistances[edge.End] = newDistance;
                        priorityQueue.Enqueue(edge.End, newDistance); // we prioritize the updated node using the priorityQueue.
                    }
                }
            }

             for (int j = 0; j < n; j++) 
                distances[i, j] = minDistances[j]; // we now set the shortest path, and we return it under for further use under since we have gotten the shortest path
        }

        return distances;
    }
}