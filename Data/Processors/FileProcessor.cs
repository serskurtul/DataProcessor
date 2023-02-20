using System;
using System.Text;
using DataProcessor.Data.Interfaces;
using DataProcessor.Data.Logs;
using DataProcessor.Data.Models;

namespace DataProcessor.Data.Processors
{
    public class FileProcessor
    {
        private readonly Mutex mut = new();
        private readonly string _outputFolderPath;
        private readonly IFileParser _parser;
        private readonly ISerializer<PaymentInfo> _serializer;
        private readonly ILogger _logger;

        public FileProcessor(string outputFolderPath, IFileParser parser, ISerializer<PaymentInfo> serializer, ILogger logger)
        {
            _outputFolderPath = outputFolderPath;
            _parser = parser;
            _serializer = serializer;
            _logger = logger;
        }

        public void Process(string filePath)
        {
            string sourceFileName = Path.GetFileName(filePath);

            var pathToSave = GetDirectory();

            _logger.Log($"Parsing file {sourceFileName}", Enums.LogType.Info);

            var result = _parser.Parse(filePath);
            if (!result.isFileValid)
            {
                _logger.Log($"File {sourceFileName} parsing info: parsed lines: {result.ParsedLines} , found errors: {result.FoundErrors} ",
                                Enums.LogType.Warning);
                _logger.Log($"filePath {sourceFileName} isn\'t valid. Passing this file", Enums.LogType.Error);
                return;
            }
            _logger.Log($"File {sourceFileName} parsing info: parsed lines: {result.ParsedLines} , found errors: {result.FoundErrors} ",
                          Enums.LogType.Warning);
            string outputFileName = $"output{GetFileNumber(pathToSave)}.json";
            string outputPath = Path.Combine(pathToSave, outputFileName);

            _logger.Log($"new file name \"{outputFileName}\"", Enums.LogType.Info);

            _logger.Log($"Serialization {sourceFileName}", Enums.LogType.Warning);

            var json = _serializer.Serialize(result.Payments);

            _logger.Log($"Serialized {sourceFileName} json lenght: {json.Length}", Enums.LogType.Info);

            using (FileStream fs = File.Create(outputPath))
            {
                _logger.Log($"file \"{outputFileName}\" has been opened", Enums.LogType.Info);

                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);

                _logger.Log($"file \"{outputFileName}\" has been writed", Enums.LogType.Info);
            }
            _logger.Log($"file \"{outputFileName}\" has been closed", Enums.LogType.Info);
            _logger.MetaLog.AddParseInfo(result);

        }
        private string GetDirectory()
        {


            var outputFolderName = DateTime.Now.ToString("dd-MM-yyyy");

            var path = Path.Combine(_outputFolderPath, outputFolderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logger.Log($"Created folder \"{outputFolderName}\" in \"{_outputFolderPath}\"", Enums.LogType.Info);
            }
            _logger.Log($"Folder \"{outputFolderName}\"in \"{_outputFolderPath}\" already exists", Enums.LogType.Info);

            return path;
        }
        private static int GetFileNumber(string folderPath)
        {
            var outputFiles = Directory.GetFiles(folderPath, "output*.json");

            var fileNumbers = outputFiles.Select(filePath =>
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                return int.Parse(fileName.Substring(6));
            });
            int maxFileNumber = (fileNumbers.Any()) ? fileNumbers.Max() : 0;

            return maxFileNumber + 1;
        }
    }
}

