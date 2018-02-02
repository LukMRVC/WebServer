using System;
using System.Net;
using System.Net.Http;

using System.Text;

namespace WebServer.Http.REST.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RestRoute : Attribute
    {
        public HttpMethod HttpMethod;
        public string Resource;
        public string Route;

        public RestRoute(HttpMethod HttpMethod, string Resource, string Route)
        {
            this.HttpMethod = HttpMethod;
            this.Resource = Resource;
            this.Route = Route;

        }


    }
}
