using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace WebServer.Model.Managers
{
    class FoodManager
    {

        public static Food AddFood(string jsonObject)
        {
            var food = JsonConvert.DeserializeObject<Food>(jsonObject);

            using (var ctx = new MenuDbContext())
            {
                ctx.Food.Add(food);
                ctx.SaveChanges();
            }

            return food;
        }

        public static IEnumerable<Food> GetFood()
        {
            IEnumerable<Food> list;
            using (var ctx = new MenuDbContext())
            {
                list = ctx.Food.ToList().OrderBy(f => f.CategoryId);
            }
            return list;
        }

    }
}
