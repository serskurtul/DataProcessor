using System;
namespace DataProcessor.Data.Models
{
    public class PaymentInfo
    {
		public string OrderFirstName { get; set; } = string.Empty;
		public string OrderLastName { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
		public decimal Payment { get; set; }
		public DateOnly Date { get; set; }
		public long AccountNumber { get; set; }
		public string Service { get; set; } = string.Empty;
    }
}

