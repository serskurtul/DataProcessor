using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using DataProcessor.Data.Interfaces;
using DataProcessor.Data.Models;
using static DataProcessor.Data.Processors.JsonSerializer.CityGroup;
using static DataProcessor.Data.Processors.JsonSerializer.CityGroup.ServiceGroup;

namespace DataProcessor.Data.Processors
{
    public class JsonSerializer : ISerializer<PaymentInfo>
    {
        public class CityGroup
        {
            public class ServiceGroup
            {
                public class Payer
                {
                    [JsonPropertyName("name")]
                    public string Name { get; set; }
                    [JsonPropertyName("payment")]
                    public decimal Payment { get; set; }
                    [JsonPropertyName("account_number")]
                    public long AccountNumber { get; set; }
                    [JsonPropertyName("date")]
                    public DateOnly Date { get; set; }
                }
                [JsonPropertyName("name")]
                public string Service { get; set; }
                [JsonPropertyName("payers")]
                public ICollection<Payer> Payers { get; set; }
                [JsonPropertyName("total")]
                public decimal Total
                {
                    get => Payers.Sum(x => x.Payment);
                }

            }
            [JsonPropertyName("city")]
            public string City { get; set; }
            [JsonPropertyName("services")]
            public ICollection<ServiceGroup> ServiceGroups { get; set; }
            [JsonPropertyName("total")]
            public decimal Total
            {
                get => ServiceGroups.Sum(x => x.Total);
            }
        }


        public string Serialize(in IEnumerable<PaymentInfo> payments)
        {
            if (payments.Count() == 0)
                return "[]";
            var groupedPayments = payments.Select(x => new
            {
                Name = x.OrderFirstName,
                Payment = x.Payment,
                City = x.Address.City,
                Service = x.Service,
                Date = x.Date,
                AccountNumber = x.AccountNumber
            })
                .GroupBy(payment => payment.City)
                .SelectMany(
                    Services => Services.GroupBy(payment => payment.Service)
                    , (City, Service) =>
                        new
                        {
                            City = City,
                            Service = Service
                        })
                .GroupBy(item => item.City.Key, item => item.Service);

            List<CityGroup> cityGroups = new List<CityGroup>();
            foreach (var group in groupedPayments)
            {
                var city = new CityGroup() { City = group.Key, ServiceGroups = new List<ServiceGroup>() };

                foreach (var serviceGroup in group)
                {
                    var service = new ServiceGroup() { Service = serviceGroup.Key, Payers = new List<Payer>() };
                    foreach (var payerGroup in serviceGroup)
                    {
                        var payer = new Payer()
                        {
                            Name = payerGroup.Name,
                            Payment = payerGroup.Payment,
                            AccountNumber = payerGroup.AccountNumber,
                            Date = payerGroup.Date
                        };

                        service.Payers.Add(payer);
                    }
                    city.ServiceGroups.Add(service);
                }

                cityGroups.Add(city);
            }

            return System.Text.Json.JsonSerializer.Serialize(cityGroups);
        }
    }
}
