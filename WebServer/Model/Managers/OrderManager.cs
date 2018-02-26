using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace WebServer.Model.Managers
{
    class OrderManager
    {

        public static Order AddOrder(string json)
        {
            var order = JsonConvert.DeserializeObject<Order>(json);
            using(var ctx = new MenuDbContext())
            {
                ctx.Order.Add(order);
                ctx.SaveChanges();
            }
            return order;
        }

        public static Order GetOrder()
        {
            Order order;
            using(var ctx = new MenuDbContext())
            {
                order = ctx.Order.LastOrDefault();
            }

            return order;
        }

        public static Order AddOrder(string json, int id)
        {
            var order = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
            var duplicates = new List<string>();
            Order realOrder;
            using (var ctx = new MenuDbContext())
            {
                var user = ctx.User.Where(u => u.Id == id).First();

                realOrder = new Order
                {
                    OrderedAt = DateTime.Now,
                    User = user,
                    UserId = id,
                    TotalPrice = Decimal.Parse(order["totalprice"][0])
                };

                realOrder.OrderFood = new List<OrderFood>();

                ctx.Order.Add(realOrder);
                
                ctx.SaveChanges();

                foreach (string foodId in order["food"])
                {
                    if (duplicates.Contains(foodId))
                        continue;

                    realOrder.OrderFood.Add(new OrderFood { Order = realOrder, Food = ctx.Food.Find(Int32.Parse(foodId)), FoodCount = order["food"].Duplicates(foodId) });
                    duplicates.Add(foodId);
                }

                ctx.SaveChanges();

            }

            return realOrder;

        }

        public static ICollection<Order> GetHistory(int userId)
        {
            ICollection<Order> orders;
            using (var ctx = new MenuDbContext())
            {
                orders = ctx.Order.Include(o => o.OrderFood)
                    .ThenInclude(of => of.Food)
                .Where(o => o.UserId == userId).OrderBy( o => o.OrderedAt).Take(10).ToArray();
                foreach(Order order in orders)
                {
                    order.Date = order.OrderedAt.ToString("dd.MM.yyyy");
                } 
            }

            return orders;
        }

    }

    static class Extension
    {
        public static int Duplicates(this string[] arr, string id)
        {
            int duplicates = 0;
            foreach (string d in arr)
            {
                if (d == id)
                    ++duplicates;
            }
            return duplicates;
        }
    }

}
