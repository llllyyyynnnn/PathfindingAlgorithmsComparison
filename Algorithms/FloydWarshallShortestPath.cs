namespace PathfindingAlgorithmsComparison.Algorithms;

public class FloydWarshallShortestPath
{
    public static double[,] Execute(Graph graph)
    {
        int n = graph.Nodes.Count; // get the nodeCount
        double[,] distances = (double[,])graph.Edges.Clone(); // we clone the graphs edges, since we don't want to manipulate the original graph
                                                              // this is because the other algorithm will use the same one

        for (int k = 0; k < n; k++) // here, we iterate through 3 different nodes in order to be able to measure different distances and then get the shortest path
        for (int i = 0; i < n; i++) // all nodes are directly based on the node count, since we have to loop through the nodes and nothing else
        for (int j = 0; j < n; j++) // they are then based off each other, so that we can get the shortest path on each run
            distances[i, j] = Math.Min(distances[i, j], distances[i, k] + distances[k, j]); // we do this by using Math.Min, which returns the smallest value, which would be the distance

        return distances; // we return for validation in the other functions, where we compare the two algorithms
    }
}