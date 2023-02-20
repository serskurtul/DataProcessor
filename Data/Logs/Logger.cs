using System;
using System.Text;
using DataProcessor.Data.Enums;
using DataProcessor.Data.Interfaces;

namespace DataProcessor.Data.Logs
{
    public class Logger : ILogger
    {
        private Timer _midnightTimer;

        private static ILogger logger;
        private MetaLog metaLog;
        private readonly string _outputFolderPath;

        private Logger(string outputFolderPath)
        {
            _outputFolderPath = outputFolderPath;
            Start();
        }

        public MetaLog MetaLog
        {
            get
            {
                if (metaLog == null)
                    metaLog = new MetaLog();
                return metaLog;
            }
        }

        public void Log(string details, LogType type)
        {
            Console.WriteLine($"[{DateTime.Now}, {type}] {details}");
        }


        public void Start()
        {
            // Set up a timer that triggers at midnight
            var now = DateTime.Now;
            var midnight = now.AddDays(1).Date;
            var timeToMidnight = midnight - now;
            _midnightTimer?.Dispose();
            _midnightTimer = new Timer(OnMidnightTimer!, null, timeToMidnight, TimeSpan.FromDays(1));

        }
        private async void OnMidnightTimer(object state)
        {
            if (metaLog == null)
                return;
            var date = DateTime.Now.AddHours(-1);
            var metaLogPath = Path.Combine(_outputFolderPath, $"{date.ToString("dd-MM-yyyy")}", "meta.log");
            using (FileStream fs = File.Create(metaLogPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(metaLog.ToString());
                await fs.WriteAsync(info, 0, info.Length);
            }
        }

        public static ILogger Init(string outputFolderPath)
        {
            if (logger == null)
                logger = new Logger(outputFolderPath);
            return logger;
        }
    }
}

