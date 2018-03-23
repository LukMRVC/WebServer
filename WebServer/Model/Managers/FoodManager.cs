using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json;

namespace WebServer.Model.Managers
{
    class FoodManager
    {

        public static Food GetFood(int id)
        {
            Food f;
            using(var ctx = new MenuDbContext())
            {
                f = ctx.Food.Where(food => food.Id == id).First();
            }
            return f;
        }


        public static Food AddFood(Food food)
        {
            using(var ctx = new MenuDbContext())
            {
                ctx.Category.Where(c => c.Id == food.CategoryId).First();
                food.FoodAllergen = new HashSet<FoodAllergen>();
                foreach (int AllergenId in food.Allergenes)
                {
                    var allergen = ctx.Allergens.Single(a => a.Id == AllergenId);
                    food.FoodAllergen.Add(new FoodAllergen(food, allergen));
                }
                ctx.Food.Add(food);
                ctx.SaveChanges();
            }
            return food;
        }


        public static Food AddFood(string jsonObject)
        {
            var food = JsonConvert.DeserializeObject<Food>(jsonObject);

            using (var ctx = new MenuDbContext())
            {
                food.FoodAllergen = new HashSet<FoodAllergen>();
                foreach (int AllergenId in food.Allergenes)
                {
                    var allergen = ctx.Allergens.Single(a => a.Id == AllergenId);
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
                list = ctx.Food.Include(f => f.FoodAllergen).ToList();
                foreach(Food f in list)
                {
                    f.Allergenes = f.FoodAllergen.Select(fa => fa.AllergenId).ToArray();
                }
            }
            return list;
        }

        //Updating this doestn work... not sure how to fix the issue
        public static Food UpdateFood(string jsonObject)
        {
            var newFood = JsonConvert.DeserializeObject<Food>(jsonObject);
            using(var ctx = new MenuDbContext())
            {

                DeleteFood(newFood.Id);
                newFood.FoodAllergen = new HashSet<FoodAllergen>();
                foreach (int AllergenId in newFood.Allergenes)
                {
                    var allergen = ctx.Allergens.Single(a => a.Id == AllergenId);
                    newFood.FoodAllergen.Add(new FoodAllergen(newFood, allergen));
                }
                ctx.Food.Add(newFood);
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
