using System.Diagnostics;

Application app = new Application();
app.Execute();

public class CsvFile // todo: move to own repo and link here
    // todo: EditContent(field, value)
{
    private struct ContentEntry
    {
        public string Content;
        public int Index;
    }

    public string Path = string.Empty; // file path
    private string _content = string.Empty; // file text content to string
    private readonly string _outputSeparator = " | ";

    private List<ContentEntry> _entries = new List<ContentEntry>(); // adding it to list for index
    private int _templateCount; // template count (x;y;z;etc at the top of the file)

    public void Initialize(string predefinedTemplate = "")
    {
        if (!File.Exists(Path)) // create file if it doesn't exist
            try
            {
                File.Create(Path).Close();
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to {Path}. ({ex.Message})", DebugFunctions.DebugType.Error);
            }

        string templateContent = string.Empty;
        if (predefinedTemplate == "") // if there's no template, use the one provided
        {
            bool inputCancelled = false;
            List<string> csvTemplate = new List<string>();

            while (!inputCancelled)
            {
                DebugFunctions.Log("Please enter the next field name for the csv template. Enter nothing to cancel.",
                    DebugFunctions.DebugType.Input);
                string input = Console.ReadLine();

                if (input.Contains(";") || input.Length == 0 || input == null)
                    DebugFunctions.Log("Invalid string.", DebugFunctions.DebugType.Error);
                else
                    csvTemplate.Add(input);

                DebugFunctions.Log("Do you want to continue? (y/n)", DebugFunctions.DebugType.Input);
                inputCancelled = Console.ReadLine()[0] == 'n';
            }

            for (int i = 0;
                 i < csvTemplate.Count;
                 i++) // since we are creating the file here for further use, we write the individual template entries one by one
            {
                string fieldName = csvTemplate[i];
                templateContent += fieldName;

                if (i < csvTemplate.Count -
                    1) // we do not add a closing semicolon at the end unless we want an invalid format
                    templateContent += ";";
            }
        }
        else
            templateContent =
                predefinedTemplate; // if template was provided, we do not need to ask for any kind of input and we can jump straight to writing it to file as it's expected to be in the correct format

        try
        {
            File.WriteAllText(Path, templateContent);
            DebugFunctions.Log("Template has been created.", DebugFunctions.DebugType.Success);
        }
        catch (Exception ex)
        {
            DebugFunctions.Log("Could not write to file.", DebugFunctions.DebugType.Error);
        }
    }

    public void Read() // assign the file entries to _entries by reading to _content and validating contents
    {
        if (File.Exists(Path)) // only try to read if the file actually exists, a wrong path could've been provided
        {
            try
            {
                _content = File.ReadAllText(Path);
                if (_content == null || _content.Length == 0)
                    throw new ArgumentException(nameof(_content), "Could not read from file.");

                if (_entries.Count > 0)
                    _entries.Clear();

                string[]
                    lines = _content.Split(Environment
                        .NewLine); // split by each new line as thats how we expect entries to be stored, with the template bein at the top
                for (int y = 0; y < lines.Length; y++) // y = entries, x = fields
                {
                    string[] fields = lines[y].Split(';');

                    if (y == 0)
                        _templateCount =
                            fields.Length; // since we are on the first line, only the csv template should be here and we can just count it in the fields

                    for (int x = 0; x < fields.Length; x++)
                    {
                        string field = fields[x];

                        ContentEntry
                            entry =
                                new ContentEntry(); // using this structure, we can assign the template index to the entry and easily access it later (for editing, accessing field name, etc)
                        entry.Content = field;
                        entry.Index = x;

                        _entries.Add(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugFunctions.Log(ex.Message, DebugFunctions.DebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.DebugType.Error);
    }

    public void Append(string content) // writes to last line of csv file
    {
        if (File.Exists(Path))
        {
            try
            {
                File.AppendAllText(Path, $"{Environment.NewLine}{content}");
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to file.", DebugFunctions.DebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.DebugType.Error);
    }

    public void
        OutputContentsToTerminal(
            bool clear =
                false) // loops through all _entries and gets largest string length for readable outputs with clear lines
    {
        if (clear)
            Console.Clear();

        List<string> largestEntries = new List<string>();
        int leftPos = 0;

        for (int i = 0;
             i < _templateCount;
             i++) // while we are adding the entries that should be defining the spacing between fields, we also make the leftpos the maximum size it can be
        {
            string largestEntry = GetLargestEntry(_entries, i);
            largestEntries.Add(largestEntry);
            leftPos += largestEntry.Length + _outputSeparator.Length;
        }

/*
         bool outputAllowed = false; // keep retrying until it has been resized (issue: will be here infinitely if the csv content is large by design TODO: FIX)

        while (!outputAllowed)
        {

        }
        */

        try // now that we have the maximum leftPos, we can attempt to set it. if the terminal window is too small compared to the output, we don't continue because then we would crash and the output wouldn't be good
        {
            Console.SetCursorPosition(leftPos, Console.GetCursorPosition().Top);
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            //outputAllowed = true;
        }
        catch (Exception ex) // this is only hit if setcursorposition failed
        {
            DebugFunctions.Log("The terminal window is too small, please resize!", DebugFunctions.DebugType.Error);
        }

        leftPos = 0;

        for (int i = 0; i < _entries.Count; i++) // read all entries, output and then add a separator
        {
            int topPosition = Console.GetCursorPosition().Top;
            ContentEntry entry = _entries[i];

            Console.Write(entry.Content);
            leftPos += largestEntries[entry.Index].Length;
            Console.SetCursorPosition(leftPos, topPosition);

            if (entry.Index < _templateCount - 1)
            {
                Console.Write(_outputSeparator);
                leftPos += _outputSeparator.Length;
            }
            else
            {
                Console.Write(Environment.NewLine);
                leftPos = 0;
            }
        }
    }

    private string
        GetLargestEntry(List<ContentEntry> entries, int index) // check all strings, replace if larger and return
    {
        string largestString = string.Empty;

        foreach (var entry in _entries.Where(e => e.Index == index))
        {
            if (entry.Content.Length > largestString.Length)
                largestString = entry.Content;
        }

        return largestString;
    }
}

public class
    DebugFunctions // simple debug outputs with categories that have different colors to make it easier on the eyes
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
                DebugFunctions.Log(
                    "Already running and reset, this could be the result of an interrupted function.",
                    DebugFunctions.DebugType.Stopwatch);
            }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void Stop(bool returnElapsedTime = false)
        {
            _stopwatch.Stop();

            if (returnElapsedTime)
                DebugFunctions.Log(
                    $"{DebugFunctions.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    DebugFunctions.DebugType.Stopwatch);
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
                DebugFunctions.Log(
                    $"{DebugFunctions.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    DebugFunctions.DebugType.Stopwatch);
        }

        public double GetElapsedMilliseconds() => _timeElapsedMilliseconds;
    }
}


public class
    Application // this is where the comparison application itself starts, the top code will get moved to other files
{
    public void Execute()
    {
    }
}