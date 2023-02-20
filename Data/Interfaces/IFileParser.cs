using DataProcessor.Data.Models;

namespace DataProcessor.Data.Interfaces
{
    public interface IFileParser
    {
        public FileParseDetails Parse(string filePath);
    }
}