using System;
using System.Collections.Generic;
using System.Linq;

namespace WebServer.Model.Managers
{
    class OrderManager
    {



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
