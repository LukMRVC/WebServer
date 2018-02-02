using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace WebServer.Http
{
    class WebServer
    {
        private Router router;
        private static HttpListener listener;

        public WebServer() {
            router = new Router();
            Start();
        }

        private static void Start() {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1234/");
            listener.Start();
            IAsyncResult result = listener.BeginGetContext(RequestCallback, listener);
            Console.WriteLine("Listening...");
        }

        public static void RequestCallback(IAsyncResult result) {
            var context = listener.EndGetContext(result);

            listener.BeginGetContext(RequestCallback, listener);

            Router.Route(context.Request, context.Response);

        }







    }
}
