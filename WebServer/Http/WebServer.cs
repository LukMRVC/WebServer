using System;
using WebServer.Model.Managers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Net;
using WebServer.Model;

namespace WebServer.Http
{
    class WebServer
    {
        private Router router;
        private static HttpListener listener;
        private static HttpListener serverSentEvents;

        public static EventWaitHandle waitHandle = new ManualResetEvent(initialState: true);

        public WebServer(string address) {
            router = new Router();
            StartEvents(address);
            Start(address);
        }

        private void StartEvents(string address)
        {
            serverSentEvents = new HttpListener();
            serverSentEvents.Prefixes.Add("http://" + address +":1234/SSE/");
            serverSentEvents.Start();
            Task.Run(() =>
            {
                while (true)
                {
                    var ctx = serverSentEvents.GetContext();
                    Task.Run( () =>
                    {
                        var response = ctx.Response;
                        response.StatusCode = 200;
                        response.ContentType = "text/event-stream";
                        response.ContentEncoding = System.Text.Encoding.UTF8;
                        try
                        {
                            while (true)
                            {
                                waitHandle.Reset();
                                string order = "data: ";
                                using (var dbCtx = new MenuDbContext())
                                {
                                    //dbCtx.Order.OrderByDescending(o => o.Id).Include( o => o.OrderFood).First()
                                    order += JsonConvert.SerializeObject(dbCtx.Order.OrderByDescending(o => o.Id)
                                        .Include(o => o.OrderFood).ThenInclude( of => of.Food ).First());
                                }
                                order += "\n\n";
                                var msg = string.Format("data: Time is now {0}\n\n", DateTime.Now);
                                var buffer = System.Text.Encoding.UTF8.GetBytes(order);
                                //response.ContentLength64 = buffer.Length;
                                response.OutputStream.Write(buffer, 0, buffer.Length);
                                response.OutputStream.Flush();
                                waitHandle.WaitOne();
                            }
                        }
                        catch (HttpListenerException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                       

                    });
                }


            });
        }

        private static void Start(string address) {
            listener = new HttpListener();
            listener.Prefixes.Add("http://" + address + ":1234/");
            listener.Start();
            IAsyncResult result = listener.BeginGetContext(RequestCallback, listener);
        }

        public static void RequestCallback(IAsyncResult result) {
            var context = listener.EndGetContext(result);

            listener.BeginGetContext(RequestCallback, listener);

            Router.Route(context.Request, context.Response);

        }







    }
}
