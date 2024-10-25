using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceTest.DataFiles
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Orders.Any())
            {
                return;
            }

            var orders = new Order[]
            {
                new Order { OrderId = "1", Weight = 10, District = "District1", DeliveryTime = DateTime.Parse("2024-10-24 10:00:00") },
                new Order { OrderId = "2", Weight = 5, District = "District1", DeliveryTime = DateTime.Parse("2024-10-24 10:15:00") },
                new Order { OrderId = "3", Weight = 8, District = "District2", DeliveryTime = DateTime.Parse("2024-10-24 10:10:00") },
                new Order { OrderId = "4", Weight = 3, District = "District1", DeliveryTime = DateTime.Parse("2024-10-24 10:35:00") },
                new Order { OrderId = "5", Weight = 7, District = "District3", DeliveryTime = DateTime.Parse("2024-10-24 11:00:00") }
            };

            foreach (Order o in orders)
            {
                context.Orders.Add(o);
            }

            context.SaveChanges();
        }
    }
}