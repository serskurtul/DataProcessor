using System;
namespace DataProcessor.Data.Models
{
	public class Address
	{
        public Address(string address)
        {
            var temp = address.Split(',', 2);
            City = temp.First();
            StreetAddress = temp.Last();
        }

        public Address()
		{
		}

		public string City { get; set; } = string.Empty;
		public string StreetAddress { get; set; } = string.Empty;

		public bool TryParse(string address)
		{
			var temp = address.Split(',', 2);
			try
			{
                City = temp.First();
                StreetAddress = temp.Last();
            }
			catch(Exception)
			{
				return false;
			}
			return true;

        }
		
		public override string ToString()
		{
			return City + ", " + StreetAddress;
		}
	}
}
