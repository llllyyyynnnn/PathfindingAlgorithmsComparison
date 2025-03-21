namespace PathfindingAlgorithmsComparison.Utilities;
using System.Diagnostics;

public class
    Debug // simple debug outputs with categories that have different colors to make it easier on the eyes
{
    public enum DebugType
    {
        Success,
        Error,
        Information,
        Warning,
        Stopwatch,
        CpuTime,
        Input
    }

    public static void Log(string message, DebugType debugType = DebugType.Information)
    {
        ConsoleColor backup = Console.ForegroundColor;

        switch (debugType)
        {
            case DebugType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("(error) ");
                break;
            case DebugType.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(success) ");
                break;
            case DebugType.Information:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("(information) ");
                break;
            case DebugType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(warning) ");
                break;
            case DebugType.Stopwatch:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("(stopwatch) ");
                break;
            case DebugType.CpuTime:
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("(cpu) ");
                break;
            case DebugType.Input:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("(input) ");
                break;
        }

        Console.ForegroundColor = backup;
        Console.WriteLine(message);
    }

    public static string GetCallerMethodName() // just for debug functions, gets the name of the calling function
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame
            callerFrame =
                stackTrace.GetFrame(2); // get the second last caller aside from the function that sent us here

        return callerFrame.GetMethod().Name;
    }
}
