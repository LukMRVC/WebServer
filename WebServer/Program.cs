using System;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Webserver handles htmls, css and basically everything we see on the web
            var server = new Http.WebServer();
            //
            Console.ReadLine();   
        }
    }
}
