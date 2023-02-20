using System;
using System.Text;
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
                    public string Name { get; set; }
                    public decimal Payment { get; set; }
                    public long AccountNumber { get; set; }
                    public DateOnly Date { get; set; }
                }
                public string Service { get; set; }
                public ICollection<Payer> Payers { get; set; }
            }
            public string City { get; set; }
            public ICollection<ServiceGroup> ServiceGroups { get; set; }
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
