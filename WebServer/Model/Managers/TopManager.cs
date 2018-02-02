using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Reflection;
using WebServer.Http.REST.Attributes;

namespace WebServer.Model.Managers
{
    class TopManager
    {

        public TopManager()
        {

        }

        public void InvokeMethod(Uri url, string httpMethod)
        {
            //Removes the "/submit" substring
            string methodName = url.AbsolutePath.Remove(0, 7);
            string[] strParams = url.Segments.Skip(2).Select(s => s.Replace("/", "")).ToArray();

            var methodToInvoke = this.GetType().GetMethods()
                .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is RestRoute && ((RestRoute)attr).Route == methodName && ((RestRoute)attr).HttpMethod.Method == httpMethod))
                .First();

            object[] parameters = methodToInvoke.GetParameters().Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType)).ToArray();

            object ret = methodToInvoke.Invoke(this, parameters);

    


        }


    }
}
