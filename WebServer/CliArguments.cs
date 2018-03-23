using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WebServer.Model;
using WebServer.Model.Managers;


namespace WebServer
{
    static class CliArguments
    {

        public static void FoodCmd(string[] args)
        {

            switch (args[1])
            {
                case "list": ListFood();
                    break;
                case "add": PromptAddFood();
                    break;
                case "remove": PromptDeleteFood();
                    break;
                default: Console.WriteLine("Invalid option: food [list, add, remove]");
                    break;
            }
        }

        private static void ListFood()
        {
            var list = FoodManager.GetFood();
            Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", "Id", "Name", "CategoryId", "Price", "Composition"));
            Console.WriteLine("---------------------");
            foreach (var f in list)
            {
                Console.WriteLine(f.ToCliString());
            }
        }

        public static void PromptAddFood()
        {
            try
            {
                Food f = new Food();
                Console.Write(@"-----------> Food name: ");
                f.Name = Console.ReadLine();
                Console.Write(@"-----------> Food gram: ");
                f.Gram = Int32.Parse(Console.ReadLine());
                Console.Write(@"-----------> Food price: ");
                f.Price = Decimal.Parse(Console.ReadLine());
                Console.Write(@"-----------> Food composition: ");
                f.Composition = Console.ReadLine();
                Console.Write(@"Category to append to: ");
                f.CategoryId = Int32.Parse(Console.ReadLine());
                Console.Write(@"-----------> Would you like to add nutritional values?[y/n]: ");
                string decision = Console.ReadLine();
                if ( decision.Equals("y", StringComparison.OrdinalIgnoreCase) || decision.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write(@"-----------> Food energy (Kj): ");
                    f.EnergyKj = Int32.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food energy (Kcal): ");
                    f.EnergyKcal = Int32.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food protein: ");
                    f.Protein = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food carbohydrates: ");
                    f.Carbohydrates = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food sugar: ");
                    f.Sugar = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food total fat: ");
                    f.TotalFat = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food saturated fat: ");
                    f.SaturatedFat = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food fiber: ");
                    f.Fiber = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food salt: ");
                    f.Salt = Decimal.Parse(Console.ReadLine());
                    Console.Write(@"-----------> Food allergenes (numbers separated by commas ','): ");
                    f.Allergenes = Console.ReadLine().Split(',').Select(s => Int32.Parse(s)).ToList();
                    FoodManager.AddFood(f);
                    Console.WriteLine("Food succesfully added");
                }
                else
                {
                    FoodManager.AddFood(f);
                    Console.WriteLine("Food succesfully added");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input! Aborting...");
                return;
            }
        }

        public static void PromptDeleteFood()
        {
            int id = 0;
            Console.Write(@"-----------> Deleted food id: ");
            try
            {
                id = Int32.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Invalid food id number");
                return;
            }
            try
            {
                Console.WriteLine(@"Food to delete: " + FoodManager.GetFood(id).ToCliString());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Given food id does not exists");
                return;
            }

            Console.Write(@"-----------> Are you sure? [y/n]: ");
            var decision = Console.ReadLine();
            if (decision.Equals("y", StringComparison.OrdinalIgnoreCase) || decision.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                CategoryManager.DeleteCategory(id);
                Console.WriteLine("Food deleted");
            }
            else
            {
                Console.WriteLine("Aborting...");
            }
        }


        public static void CategoryCmd(string[] args)
        {
            switch (args[1])
            {
                case "list": ListCategories();
                    break;
                case "add": PromptAddCategory();
                    break;
                case "remove": PromptDeleteCategory();
                    break;
                default:
                    Console.WriteLine(@"Invalid option: category [list, add, remove]");
                    break;
            }
        }


        private static void ListCategories()
        {
            var list = CategoryManager.GetCategories();
            Console.WriteLine(string.Format(@"{0} {1} {2}", @"Id", @"Name", @"ParentId"));
            Console.WriteLine(@"---------------------");
            foreach (Category c in list)
            {
                Console.WriteLine(c.ToCliString());
            }
        }

        private static void PromptAddCategory()
        {
            string name;
            int parentId = 0;
            Console.Write(@"-----------> Category name: ");
            name = Console.ReadLine();
            Console.Write(@"-----------> Parent Category Id: ");
            try
            {
                parentId = Int32.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Invalid parent category id number");
                return;
            }
            Category c = new Category(name, parentId);
            try
            {
                var cat = CategoryManager.AddCategory(c);
                Console.WriteLine(@"Successfully added new category " + cat.ToCliString());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Error while adding a new category: Parent Id does not exist!");
            }
            
        }

        private static void PromptDeleteCategory()
        {
            int id = 0;
            Console.Write(@"-----------> Deleted category id: ");
            try
            {
                id = Int32.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Invalid category id number");
                return;
            }
            try
            {
                Console.WriteLine(@"Category to delete: " + CategoryManager.GetCategory(id).ToCliString());
            }
            catch (Exception)
            {
                Console.WriteLine(@"Given category id does not exists");
                return;
            }

            Console.Write(@"-----------> Are you sure? [y/n]: ");
            var decision = Console.ReadLine();
            if (decision.Equals("y", StringComparison.OrdinalIgnoreCase) || decision.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                CategoryManager.DeleteCategory(id);
                Console.WriteLine("Category deleted");
            }
            else
            {
                Console.WriteLine("Aborting...");
            }
        }

    }
}
