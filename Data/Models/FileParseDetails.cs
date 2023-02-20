using System;
namespace DataProcessor.Data.Models
{
    public class FileParseDetails
    {
        public FileParseDetails(string fileName)
        {
            FileName = fileName;
        }
        public string FileName { get; init; }
        public bool isFileValid
        {
            get {
                if(ParsedLines == FoundErrors)
                    return false;
                return true;
            }
        }
        public int ParsedLines { get; set; } = 0;
        public int FoundErrors { get; set; } = 0;

        public ICollection<PaymentInfo> Payments { get; set; } = new List<PaymentInfo>();
    }
}

