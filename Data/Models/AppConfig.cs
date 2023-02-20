using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataProcessor.Data.Models
{
	public class AppConfig
	{
        public AppConfig(string configPath)
        {
            LoadConfig(configPath);
        }
        [JsonConstructor]
        public AppConfig(string? sourceFolderPath, string? outputFolderPath)
        {
            SourceFolderPath = sourceFolderPath;
            OutputFolderPath = outputFolderPath;
        }

        public string? SourceFolderPath { get; set; }
        public string? OutputFolderPath { get; set; }

        private void LoadConfig(string configPath)
        {
            string json = File.ReadAllText(configPath);
            var settings = JsonSerializer.Deserialize<AppConfig>(json);
            OutputFolderPath = settings!.OutputFolderPath;
            SourceFolderPath = settings!.SourceFolderPath;

        }
    }
}