namespace PathfindingAlgorithmsComparison.Utilities;
using System.Diagnostics;

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
                Debug.Log($"Could not write to {Path}. ({ex.Message})", Debug.DebugType.Error);
            }

        string templateContent = string.Empty;
        if (predefinedTemplate == "") // if there's no template, use the one provided
        {
            bool inputCancelled = false;
            List<string> csvTemplate = new List<string>();

            while (!inputCancelled)
            {
                Debug.Log("Please enter the next field name for the csv template. Enter nothing to cancel.",
                    Debug.DebugType.Input);
                string input = Console.ReadLine();

                if (input.Contains(";") || input.Length == 0 || input == null)
                    Debug.Log("Invalid string.", Debug.DebugType.Error);
                else
                    csvTemplate.Add(input);

                Debug.Log("Do you want to continue? (y/n)", Debug.DebugType.Input);
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
            Debug.Log("Template has been created.", Debug.DebugType.Success);
        }
        catch (Exception ex)
        {
            Debug.Log("Could not write to file.", Debug.DebugType.Error);
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
                Debug.Log(ex.Message, Debug.DebugType.Error);
            }
        }
        else
            Debug.Log($"{Path} does not exist.", Debug.DebugType.Error);
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
                Debug.Log($"Could not write to file.", Debug.DebugType.Error);
            }
        }
        else
            Debug.Log($"{Path} does not exist.", Debug.DebugType.Error);
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
        
        try // now that we have the maximum leftPos, we can attempt to set it. if the terminal window is too small compared to the output, we don't continue because then we would crash and the output wouldn't be good
        {
            Console.SetCursorPosition(leftPos, Console.GetCursorPosition().Top);
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            //outputAllowed = true;
        }
        catch (Exception ex) // this is only hit if setcursorposition failed
        {
            Debug.Log("The terminal window is too small, please resize!", Debug.DebugType.Error);
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