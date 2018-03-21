using System;
using WebServer.Model;
using System.Linq;
using clipr;

namespace WebServer
{
    class Program
    {
        public static string ConnString { get; set; }
        static void Main(string[] args)
        {
            string add = "";
            Options opt;
            try
            {
                opt = CliParser.Parse<Options>(args);
                ConnString = string.Format("server={0};port=3306;database={1};uid={2};password={3};charset=utf8;persistsecurityinfo=True", opt.Host, opt.Database, opt.User, opt.Password);
                add = opt.Address;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(-1);
            }

            try
            {
                using (var ctx = new MenuDbContext())
                {
                    ctx.Database.EnsureCreated();
                    var result = ctx.Allergens.FirstOrDefault();

                    if (result == null)
                    {
                        foreach (string allergen in ctx.allergenValues)
                        {
                            ctx.Allergens.Add(new Allergen(allergen));
                        }
                        ctx.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(-1);
            }
            
            //Webserver handles htmls, css and basically everything we see on the web
            try
            {
                var server = new Http.WebServer(add);

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.Exit(-1);
            }
            
            string command = "";
            do
            {
                Console.Write("webserver> ");
                command = Console.ReadLine();
            } while (!command.Equals("stop", StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
