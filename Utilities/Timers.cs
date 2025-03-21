namespace PathfindingAlgorithmsComparison.Utilities;
using System.Diagnostics;

public class Timers // stopwatch & cpu time can be measured with these classes and their functions
{
    public class Stopwatch
    {
        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        public void Start()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                Debug.Log(
                    "Already running and reset, this could be the result of an interrupted function.",
                    Debug.DebugType.Stopwatch);
            }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void Stop(bool returnElapsedTime = false)
        {
            _stopwatch.Stop();

            if (returnElapsedTime)
                Debug.Log(
                    $"{Debug.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    Debug.DebugType.Stopwatch);
        }

        public double GetElapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;
    }

    public class CPU
    {
        private double _storedTime;
        private double _timeElapsedMilliseconds;

        public void Start() => _storedTime = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds;

        public void Stop(bool returnElapsedTime = false)
        {
            _timeElapsedMilliseconds = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds - _storedTime;

            if (returnElapsedTime)
                Debug.Log(
                    $"{Debug.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    Debug.DebugType.Stopwatch);
        }

        public double GetElapsedMilliseconds() => _timeElapsedMilliseconds;
    }
}