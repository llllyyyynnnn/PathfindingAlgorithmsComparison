﻿using PathfindingAlgorithmsComparison.Utilities;
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

    private int
        algorithmsMaxIncrements =
            10; // the amount of times we want to increment by nodesIncrementAmount, to test performance with smaller & larger graphs

    private int nodesIncrementAmount = 100;

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

    public class
        TestResults // each result will be returned using this class, so that we can add it to a list and save it to csv for later analysis
    {
        public required string AlgorithmName;
        public required double StopwatchElapsedMilliseconds;
        public required double CpuElapsedMilliseconds;
        public required int NodesAmount;
    }

    public TestResults CreateTestResults(string algorithmName, double stopwatchElapsedMilliseconds,
        double cpuElapsedMilliseconds, int nodesAmount)
    {
        TestResults results = new TestResults
        {
            AlgorithmName = algorithmName,
            StopwatchElapsedMilliseconds = _stopwatch.GetElapsedMilliseconds(),
            CpuElapsedMilliseconds = _cpu.GetElapsedMilliseconds(),
            NodesAmount = nodesAmount
        };

        return results;
    }

    public CsvFile
        WriteResultsToCsv(string path,
            List<TestResults> results) // if the file already exists, we just append the entries. else, we initialize it using the predefined template
    {
        CsvFile file = new CsvFile();
        file.Path = path;
        if (!File.Exists(path)) file.Initialize("Algorithm;Stopwatch;Cpu;Nodes");

        foreach (TestResults res in results)
        {
            file.Append(
                $"{res.AlgorithmName};{res.StopwatchElapsedMilliseconds} ms;{res.CpuElapsedMilliseconds} ms;{res.NodesAmount}");
        }

        return file;
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
            Graph
                graph = new Graph(nodesIncrementAmount * i,
                    50); // here, we create the graph that we're going to be using for the algorithms with x nodes and y percent (which determines the amount of possible edges)

            StartTimers();
            double[,]
                floydDistances =
                    FloydWarshallShortestPath
                        .Execute(graph); // run floyd warshall's algorithm and store it in a variable 
            StopTimers();

            double floydStopwatchTime = _stopwatch.GetElapsedMilliseconds();
            double floydCpuTime = _cpu.GetElapsedMilliseconds();
            Console.WriteLine("Floyd Warshalls algorithm has finished running");

            StartTimers();
            double[,]
                dijkstraDistances =
                    DijkstraShortestPath
                        .Execute(graph); // same as in the above one, we store the variable for interpretation
            StopTimers();

            double dijkstraStopwatchTime = _stopwatch.GetElapsedMilliseconds();
            double
                dijkstraCpuTime =
                    _cpu.GetElapsedMilliseconds(); // we cache the variables here so we can add them to TestResults later, without the need of creating 2x timer objects
            Console.WriteLine("Dijkstra algorithm has finished running");

            try
            {
                VerifyDistances(floydDistances,
                    dijkstraDistances); // verify to make sure the algorithms are functioning as expected, we do this in try & catch statements since we return an exception if it fails
                floydAlgorithmResults.Add(new TestResults
                {
                    AlgorithmName = "Floyd Warshall",
                    StopwatchElapsedMilliseconds = floydStopwatchTime,
                    CpuElapsedMilliseconds = floydCpuTime,
                    NodesAmount = nodesIncrementAmount * i
                });

                dijkstraAlgorithmResults.Add(new TestResults
                {
                    AlgorithmName = "Dijkstra",
                    StopwatchElapsedMilliseconds = dijkstraStopwatchTime,
                    CpuElapsedMilliseconds = dijkstraCpuTime,
                    NodesAmount = nodesIncrementAmount * i
                }); // add results to individual lists so we can sort them later in the csv file and have it consistent

                //Console.WriteLine("The algorithms have been verified and match within a valid range."); // To avoid clutter, this is commented out since it doesn't contribute much. If something goes wrong, we mention it. Else, we don't.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to verify: {ex.Message}");
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