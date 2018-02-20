using System;
using Microsoft.EntityFrameworkCore;
using WebServer.Model;
using System.Linq;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {

            using(var ctx = new MenuDbContext())
            {
                ctx.Database.EnsureCreated();
                var result = ctx.Allergens.FirstOrDefault();

                if(result == null)
                {
                    foreach(string allergen in ctx.allergenValues){
                        ctx.Allergens.Add(new Allergen(allergen));
                    }
                    ctx.SaveChanges();
                }

            }



            //Webserver handles htmls, css and basically everything we see on the web
            try
            {
                var server = new Http.WebServer();

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //
            Console.ReadLine();   
        }
    }
}
