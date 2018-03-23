using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebServer.Model.Managers
{
    class CategoryManager
    {


        public static Category AddCategory(Category c)
        {
            using (var ctx = new MenuDbContext())
            {
                //Test if parentId exists
                ctx.Category.Where(cat => cat.Id == c.ParentId).First();
                ctx.Category.Add(c);
                ctx.SaveChanges();
            }
            return c;
        }

        public static Category GetCategory(int id)
        {
            Category cat;
            using(var ctx = new MenuDbContext())
            {
                cat = ctx.Category.Where(c => c.Id == id).First();
            }
            return cat;
        }

        public static Category AddCategory(string postText)
        {
            var categoryText = JsonConvert.DeserializeObject<Dictionary<string, string>>(postText);
            Category newCategory;
            //var category = JsonConvert.DeserializeObject<Category>(postText);
            using (var ctx = new MenuDbContext())
            {
                
                if (categoryText["ParentName"].Equals("Vybrat...", StringComparison.CurrentCultureIgnoreCase))
                {
                    newCategory = new Category(categoryText["Name"], null);
                }
                else
                {
                    var parent = (from c in ctx.Category.ToList() where c.Name == categoryText["ParentName"] select c).First();
                    newCategory = new Category(categoryText["Name"], parent.Id);
                }
                ctx.Category.Add(newCategory);
                ctx.SaveChanges();
            }
            return newCategory;
        }


        public static IEnumerable<Category> GetCategories()
        {
            IEnumerable < Category > list;
            using (var ctx = new MenuDbContext())
            {
                list = ctx.Category.ToList().OrderBy(c => c.ParentId);
            }
            return list;
        }

        public static void DeleteCategory(int id)
        {
            using(var ctx = new MenuDbContext())
            {
                var toRemove = ctx.Category.FirstOrDefault(c => c.Id == id);
                ctx.Category.Remove(toRemove);
                var toRemoveChild = ctx.Category.Where(c => c.ParentId == id);
                ctx.Category.RemoveRange(toRemoveChild);
                ctx.SaveChanges();
            }
        }

    }
}
