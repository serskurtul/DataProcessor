using System;
using System.Globalization;
using DataProcessor.Data.Interfaces;
using DataProcessor.Data.Models;

namespace DataProcessor.Data.Processors
{
    // John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water
    public class PaymentParser : IPaymentParser
    {
        private readonly ILogger _logger;

        public PaymentParser(ILogger logger)
        {
            _logger = logger;
        }
        public bool TryParse(string? info, ref PaymentInfo? paymentInfo)
        {
            if (string.IsNullOrEmpty(info))
                return false;
            string[] infoElements = Split(info).ToArray();
            _logger.Log($"Розрізнило :({string.Join(", ", infoElements)})", Enums.LogType.Info);
            if (infoElements.Length != 7)
                return false;
            long accountNumber;
            decimal payment;
            DateOnly date;
            ClearStr(ref infoElements[4]);
            ClearStr(ref infoElements[3]);
            ClearStr(ref infoElements[5]);
            _logger.Log("Trying to parse: payment, date, account number", Enums.LogType.Info);

            if (!decimal.TryParse(infoElements[3], CultureInfo.InvariantCulture, out payment)
                || !DateOnly.TryParseExact(infoElements[4], "yyyy-dd-MM", out date)
                || !TryParse(out accountNumber, infoElements[5]))
            {
                _logger.Log("Parse payment, date, account number FAILED", Enums.LogType.Error);
                return false;
            }
            _logger.Log("Parse payment, date, account number successful", Enums.LogType.Info);
            ClearStr(ref infoElements[0]);
            ClearStr(ref infoElements[1]);
            ClearStr(ref infoElements[6]);

            paymentInfo = new PaymentInfo()
            {
                OrderFirstName = infoElements[0],
                OrderLastName = infoElements[1],
                Service = infoElements[6],
                AccountNumber = accountNumber,
                Payment = payment,
                Date = date
            };
            ClearStr(ref infoElements[2]);
            _logger.Log("Trying to pars address", Enums.LogType.Warning);

            if (!paymentInfo.Address.TryParse(infoElements[2]))
            {
                _logger.Log("Parse address FAILED", Enums.LogType.Error);
                return false;
            }
            _logger.Log("Parse address successful", Enums.LogType.Info);

            return true;
        }

        private static bool TryParse<T>(out T obj, string str) where T : IParsable<T>
        {
            ClearStr(ref str);
            return T.TryParse(str, null, out obj!);
        }
        private static void ClearStr(ref string str)
        {
            str = str.Replace("\"", "");
            str = str.Replace(" ", "");
            str = str.Replace("“", "");
            str = str.Replace("”", "");
        }

        private static IEnumerable<string> Split(string str)
        {
            List<string> tokens = new List<string>();
            int startPosition = 0;
            bool isInQuotes = false;
            for (int currentPosition = 0; currentPosition < str.Length; currentPosition++)
            {
                if (str[currentPosition] == '\"' || str[currentPosition] == '“' || str[currentPosition] == '”')
                {
                    isInQuotes = !isInQuotes;
                }
                else if (str[currentPosition] == ',' && !isInQuotes)
                {
                    tokens.Add(str.Substring(startPosition, currentPosition - startPosition));
                    startPosition = currentPosition + 1;
                }
            }
            tokens.Add(str.Substring(startPosition));

            return tokens;
        }
    }
}