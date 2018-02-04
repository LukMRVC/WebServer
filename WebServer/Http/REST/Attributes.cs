using System;
using System.Net;
using System.Net.Http;

using System.Text;

namespace WebServer.Http.REST.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class RestRoute : Attribute
    {
        public string HttpMethod;
        public string Route;

        public RestRoute(string Route, string HttpMethod)
        {
            this.HttpMethod = HttpMethod;
            this.Route = Route;

        }


    }
}
