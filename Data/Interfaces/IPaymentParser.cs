using System;
using DataProcessor.Data.Models;

namespace DataProcessor.Data.Interfaces
{
	public interface IPaymentParser
	{
        bool TryParse(string? info, ref PaymentInfo? paymentInfo);
	}
}

