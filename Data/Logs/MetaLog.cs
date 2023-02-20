using DataProcessor.Data.Models;

namespace DataProcessor.Data.Logs
{
    public class MetaLog
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public List<string> InvalidFiles { get; set; } = new List<string>();

        public void AddParseInfo(FileParseDetails fileDetails)
        {
            ParsedFiles++;

            if (fileDetails.isFileValid)
            {
                ParsedLines += fileDetails.ParsedLines;
                FoundErrors += fileDetails.FoundErrors;
            }
            else
            {
                InvalidFiles.Add(fileDetails.FileName);
            }
        }

        public static MetaLog FromFileParseDetailsList(List<FileParseDetails> fileDetailsList)
        {
            var metaLog = new MetaLog
            {
                ParsedFiles = fileDetailsList.Count,
                ParsedLines = fileDetailsList.Sum(f => f.ParsedLines),
                FoundErrors = fileDetailsList.Sum(f => f.FoundErrors),
                InvalidFiles = fileDetailsList.Where(f => !f.isFileValid).Select(f => f.FileName).ToList()
            };

            return metaLog;
        }

        public override string ToString()
        {
            return $"parsed_files: {ParsedFiles}\n" +
                   $"parsed_lines: {ParsedLines}\n" +
                   $"found_errors: {FoundErrors}\n" +
                   $"invalid_files: [{string.Join(", ", InvalidFiles)}]";
        }
    }
}

