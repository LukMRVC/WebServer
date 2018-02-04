using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebServer.Model.Managers
{
    class CategoryManager
    {

        public static void AddCategory(string postText)
        {
            var category = JsonConvert.DeserializeObject<Category>(postText);
            using (var ctx = new MenuDbContext())
            {
                ctx.Category.Add(category);
                ctx.SaveChanges();
            }
        }


        public static IEnumerable<Category> GetCategories()
        {
            IEnumerable < Category > list;
            using (var ctx = new MenuDbContext())
            {
                list = ctx.Category.ToList();
            }
            return list;
        }

    }
}
