using System.Diagnostics;
using DataProcessor.Data.Interfaces;
using DataProcessor.Data.Logs;
using DataProcessor.Data.Models;
using DataProcessor.Data.Processors;



internal class Program
{
    
    private static void Main(string[] args)
    {
        AppConfig config = new AppConfig("config.json");
        if (config.OutputFolderPath == null || config.SourceFolderPath == null)
        {
            Console.WriteLine("Problems with the config.json file. Missing values or other");
            return;
        }
            
        var logger = Logger.Init(config.OutputFolderPath);

        FileSystemWatcher watcher = new FileSystemWatcher()
        {
            Path = config.SourceFolderPath,
            IncludeSubdirectories = true,
            Filters = {"*.txt", "*.csv"},
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName

        };
        FileSystemEventHandler OnStart = (sender, e) =>
        {
            IFileParser parser = new FileParser(new PaymentParser());
            ISerializer<PaymentInfo> serializer = new JsonSerializer();

            var a = new FileProcessor(config.OutputFolderPath, parser, serializer, logger);
            a.Process(e.FullPath);
        };
        watcher.Created += OnStart;

        var options = new List<Option> {
            new Option("Reset", ()=>
            {
                watcher.Dispose();
                watcher = new FileSystemWatcher()
                {
                    Path = config.SourceFolderPath,
                    IncludeSubdirectories = true,
                    Filters = {"*.txt", "*.csv"},
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.FileName
                };
                watcher.Created += OnStart;

            }),
            new Option("Stop", ()=>
            {
                Environment.Exit(0);
            })
        };

        int index = 0;
        WriteMenu(options, options[index]);
        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.DownArrow)
            {
                if (index + 1 < options.Count)
                {
                    index++;
                    WriteMenu(options, options[index]);
                }
            }
            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                if (index - 1 >= 0)
                {
                    index--;
                    WriteMenu(options, options[index]);
                }
            }
            if (keyinfo.Key == ConsoleKey.Enter)
            {
                options[index].action.Invoke();
                index = 0;
            }
        }
        while (keyinfo.Key != ConsoleKey.X);
        Console.ReadKey();
    }
    static void OnStart(object sender, FileSystemEventArgs e)
    {

    }
    static void WriteMenu(List<Option> options, Option selectedOption)
    {
        Console.Clear();

        foreach (Option option in options)
        {
            if (option == selectedOption)
            {
                Console.Write("> ");
            }
            else
            {
                Console.Write(" ");
            }

            Console.WriteLine(option.Name);
        }
    }
}

class Option
{
    public string Name { get; }
    public Action action { get; }

    public Option(string name, Action action)
    {
        Name = name;
        this.action = action;
    }
}