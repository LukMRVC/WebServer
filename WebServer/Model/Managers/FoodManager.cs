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
                food.FoodAllergen = new HashSet<FoodAllergen>();
                foreach (int AllergenId in food.Allergenes)
                {
                    var allergen = (from a in ctx.Allergens.ToList() where a.Id == AllergenId select a).First();
                    food.FoodAllergen.Add(new FoodAllergen(food, allergen));
                }
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

        //Updating this doestn work... not sure how to fix the issue
        public static Food UpdateFood(string jsonObject)
        {
            var newFood = JsonConvert.DeserializeObject<Food>(jsonObject);
            using(var ctx = new MenuDbContext())
            {
                //Solve detached M:N relations
                // var old = ctx.Food.Where(f => f.Id == newFood.Id).First();
                //newFood.FoodAllergen = ctx.Food.Where(f => f.Id == newFood.Id).First().FoodAllergen;
                newFood.FoodAllergen = new HashSet<FoodAllergen>();
                foreach(int AllergenId in newFood.Allergenes)
                {
                    var allergen = (from a in ctx.Allergens.ToList() where a.Id == AllergenId select a).First();
                    newFood.FoodAllergen.Add(new FoodAllergen(newFood, allergen));
                }
                ctx.Food.Update(newFood);
                ctx.SaveChanges();
            }

            return newFood;
        }

        public static void DeleteFood(int id)
        {
            using(var ctx = new MenuDbContext())
            {
                var toRemove = ctx.Food.FirstOrDefault(f => f.Id == id);
                ctx.Food.Remove(toRemove);
                ctx.SaveChanges();
            }
        }
    }
}
