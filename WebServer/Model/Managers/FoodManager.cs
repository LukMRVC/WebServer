using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WebServer.Model.Managers
{
    class FoodManager
    {

        public static void AddFood(string jsonObject)
        {
            var food = JsonConvert.DeserializeObject<Food>(jsonObject);

            using (var ctx = new MenuDbContext())
            {
                ctx.Food.Add(food);
                ctx.SaveChanges();
            }
        }
    }
}
