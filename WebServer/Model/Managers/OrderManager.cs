using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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


    }
}
