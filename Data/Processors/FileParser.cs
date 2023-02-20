using System;
using DataProcessor.Data.Interfaces;
using DataProcessor.Data.Models;

namespace DataProcessor.Data.Processors
{
    public class FileParser : IFileParser
    {
        private readonly IPaymentParser _parser;

        public FileParser(IPaymentParser parser)
        {
            _parser = parser;
        }

        public FileParseDetails Parse(string filePath)
        {
            FileParseDetails fileParseDetails = new FileParseDetails(filePath);
            bool passFirtsLine = Path.GetExtension(filePath) == ".csv";
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    PaymentInfo? paymentInfo = null;
                    string? paymentStr = reader.ReadLine();
                    if (!passFirtsLine && !string.IsNullOrEmpty(paymentStr))
                    {
                        bool isParsed = _parser.TryParse(paymentStr, ref paymentInfo);
                        fileParseDetails.ParsedLines++;
                        if (isParsed)
                        {
                            fileParseDetails.Payments.Add(paymentInfo!);
                        }
                        else
                        {
                            fileParseDetails.FoundErrors++;
                        }
                    }
                    else
                        passFirtsLine = false;
                }
            }

            return fileParseDetails;
        }
    }
}

