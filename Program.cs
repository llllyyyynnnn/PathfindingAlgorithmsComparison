using PathfindingAlgorithmsComparison.Utilities;
using PathfindingAlgorithmsComparison.Algorithms;

Application app = new Application();
app.Execute();

public class
    Application // this is where the comparison application itself starts, the top code will get moved to other files
{
    /// <summary>
    ///  Compare distances between the two algorithms.
    /// </summary>
    /// <param name="a">the distances from algorithm a.</param>
    /// <param name="b">the distances from algorithm b.</param>
    /// <exception cref="ArgumentException">indicates that the distances are not ok.</exception>
    private static void VerifyDistances(double[,] a, double[,] b)
    {
        const double maxOk = 0.5;

        if (a == null || b == null || a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        {
            throw new ArgumentException("the distance matrices don't have the same sizes.");
        }

        for (int r = 0; r < a.GetLength(0); r++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                if (a[r, c] - b[r, c] > maxOk || b[r, c] - a[r, c] > maxOk)
                {
                    throw new ArgumentException("a[" + r + ", " + c + "] is not equal to " + "b[" + r + ", " + c +
                                                "]. " + "The difference is " + (a[r, c] - b[r, c]) + ".");
                }
            }
        }
    }

    private Timers.Stopwatch
        _stopwatch = new Timers.Stopwatch(); // store the timers here, they will reset whenever .Start() is executed

    private Timers.CPU _cpu = new Timers.CPU();

    private int algorithmsMaxIncrements = 8; // the amount of times we want to increment by nodesIncrementAmount, to test performance with smaller & larger graphs
    private int repeatPerGraph = 4;
    private int nodesIncrementAmount = 100;
    private int nodePercentageBase = 20;
    private int nodePercentageMaximum = 80;
    private int nodePercentageIncrement = 15;

    private void StartTimers()
    {
        _stopwatch.Start();
        _cpu.Start();
    }

    private void StopTimers()
    {
        _stopwatch.Stop();
        _cpu.Stop();
        Console.WriteLine($"Stopwatch: {_stopwatch.GetElapsedMilliseconds()} ms");
        Console.WriteLine($"CPU: {_cpu.GetElapsedMilliseconds()} ms");
    }

    public class TestResults // each result will be returned using this class, so that we can add it to a list and save it to csv for later analysis
    {
        public required string AlgorithmName;
        public required double StopwatchElapsedMilliseconds;
        public required double CpuElapsedMilliseconds;
        public required int NodesAmount;
        public required double NodesPercentage;
        public required int repeatPerGraph;
        public required string BuildConfiguration;
    }

    public TestResults CreateTestResults(string algorithmName, double stopwatchElapsedMilliseconds, double cpuElapsedMilliseconds, int nodesAmount, double nodesPercentage)
    {
        TestResults results = new TestResults
        {
            AlgorithmName = algorithmName,
            StopwatchElapsedMilliseconds = stopwatchElapsedMilliseconds,
            CpuElapsedMilliseconds = cpuElapsedMilliseconds,
            NodesAmount = nodesAmount,
            NodesPercentage = nodesPercentage,
            repeatPerGraph = repeatPerGraph,
#if DEBUG
            BuildConfiguration = "Debug"
#else // assume it's on release (since we only have 2, for now at least), if there's more than 1 defined configuration then it will not work as everything that isnt debug is then release
            BuildConfiguration = "Release"
#endif
        };

        return results;
    }
    public CsvFile WriteResultsToCsv(string path, List<TestResults> results) // if the file already exists, we just append the entries. else, we initialize it using the predefined template
    {
        CsvFile file = new CsvFile();
        file.Path = path;
        if (!File.Exists(path)) file.Initialize("Algorithm;Stopwatch;Cpu;Nodes;Percentage; Repeated; Build Configuration");

        foreach (TestResults res in results)
        {
            file.Append(
                $"{res.AlgorithmName};{res.StopwatchElapsedMilliseconds} ms;{res.CpuElapsedMilliseconds} ms;{res.NodesAmount}; {res.NodesPercentage}%; {res.repeatPerGraph}; {res.BuildConfiguration}");
        }

        return file;
    }

    private void RunPathfindingAlgorithms(List<TestResults> floydTestResults, List<TestResults> dijkstraTestResults, Graph graph, int nodeAmount, int nodePercentage) {
        List<double[,]> distancesFloyd = new List<double[,]>();
        List<double[,]> distancesDijkstra = new List<double[,]>();

        StartTimers();
        for (int i = 0; i < repeatPerGraph; i++)
            distancesFloyd.Add(FloydWarshallShortestPath.Execute(graph));
        StopTimers();

        double floydStopwatchTime = _stopwatch.GetElapsedMilliseconds();
        double floydCpuTime = _cpu.GetElapsedMilliseconds();
        Console.WriteLine($"Floyd Warshalls algorithm has finished running");

        StartTimers();
        for (int i = 0; i < repeatPerGraph; i++)
            distancesDijkstra.Add(DijkstraShortestPath.Execute(graph));
        StopTimers();

        double dijkstraStopwatchTime = _stopwatch.GetElapsedMilliseconds();
        double dijkstraCpuTime = _cpu.GetElapsedMilliseconds();
        Console.WriteLine($"Dijkstras algorithm has finished running");
        Console.WriteLine($"Algorithms have finished running {repeatPerGraph} times for {nodeAmount}, {nodePercentage}%");

        try
        {
            if (distancesFloyd.Count == distancesDijkstra.Count) { // they should always be matching
                for (int i = 0; i < distancesFloyd.Count; i++)
                    VerifyDistances(distancesFloyd[i], distancesDijkstra[i]);
            }
            else
            {
                throw new Exception("Lists did not match.");
            }

            floydTestResults.Add(CreateTestResults("Floyd Warshall", floydStopwatchTime, floydCpuTime, nodeAmount, nodePercentage));
            dijkstraTestResults.Add(CreateTestResults("Dijkstra", dijkstraStopwatchTime, dijkstraCpuTime, nodeAmount, nodePercentage)); // add results to individual lists so we can sort them later in the csv file and have it consistent
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to verify: {ex.Message}");
        }
    }

    public void Execute()
    {
        List<TestResults> floydAlgorithmResults = new List<TestResults>();
        List<TestResults> dijkstraAlgorithmResults = new List<TestResults>();

        Console.WriteLine("Please enter the file name to save the csv content to.");
        string fileName = Console.ReadLine();
        while
            (fileName == null ||
             fileName.Length <= 0) // don't allow the function to continue unless we have a valid filename
        {
            Console.WriteLine("Invalid input.");
            fileName = Console.ReadLine();
        }

        for (int i = 1;
             i < algorithmsMaxIncrements + 1;
             i++) // + 1, we don't want to start from 0 since then we'll have 0 nodes
        {
            int currentNodePercentage = nodePercentageBase;
            int currentNodeAmount = nodesIncrementAmount * i;

            while (currentNodePercentage <= nodePercentageMaximum)
            {
                Graph currentGraph = new Graph(currentNodeAmount, currentNodePercentage);
                RunPathfindingAlgorithms(floydAlgorithmResults, dijkstraAlgorithmResults, currentGraph, currentNodeAmount, currentNodePercentage); // run the same function for each graph, saves to our defined lists at the top of the function and we then save it to csv

                if (currentNodePercentage + nodePercentageIncrement > nodePercentageMaximum && currentNodePercentage != nodePercentageMaximum) // clamp the value if we reach the max
                    currentNodePercentage = nodePercentageMaximum;
                else
                    currentNodePercentage += nodePercentageIncrement;
            }

            Console.WriteLine($"---- {i}/{algorithmsMaxIncrements} have been executed ----");
        }

        if (floydAlgorithmResults.Count > 0 &&
            dijkstraAlgorithmResults.Count >
            0) // only do this if we havent ran into any issues above, indicated by the fact we have more than 0 count
        {
            WriteResultsToCsv(fileName, floydAlgorithmResults);
            WriteResultsToCsv(fileName,
                dijkstraAlgorithmResults); // write results, it's appended so its fine to call it twice
        }
    }
}