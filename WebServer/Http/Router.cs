using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using WebServer.Model;
using WebServer.Model.Managers;
using WebServer.Http.REST.Attributes;
using WebServer.Http.REST;
using Newtonsoft.Json;

namespace WebServer.Http
{
    class Router
    {

        private RsaCryption cryption;

        private static Router reference;

        public Router()
        {
            cryption = new RsaCryption();
            Token.Cryption = cryption;
            reference = this;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static Stream GenerateErrorPage(HttpStatusCode statusCode) {
            string error = statusCode.ToString();
            
            string html = string.Format("<head><title>{0} - {1}</title></head>" +
                "<body><h1>{0} - {1}</h1></body>",
                (int)statusCode, error);

            return GenerateStreamFromString(html);
        }

        public static Stream GenerateErrorPage(HttpStatusCode statusCode, string message)
        {
            string error = statusCode.ToString();

            string html = string.Format("<head><title>{0} - {1}</title></head>" +
                "<body><h1>{0} - {1}</h1><h2>{2}</h2></body>",
                (int)statusCode, error, message);

            return GenerateStreamFromString(html);
        }


        public static void Route(HttpListenerRequest request, HttpListenerResponse HttpResponse)
        {
            //Router.HttpResponse = HttpResponse;
            ResponseObject responseObject = new ResponseObject();
            if (request.Url.AbsolutePath.Contains("api"))
            {
                if (request.HasEntityBody)
                {
                    responseObject = reference.InvokeMethod(request.Url, request.HttpMethod, request.InputStream);

                }
                else
                {
                    responseObject = reference.InvokeMethod(request.Url, request.HttpMethod);
                }
                //response.Redirect(@"http://localhost:1234/");
            }
            else
            {
                try
                {
                    //Gets file content
                    responseObject = FileFinder.GetFile(request.Url.AbsolutePath.Substring(1));
                    if (responseObject.Content == null)
                    {
                        responseObject.Content = GenerateErrorPage(HttpStatusCode.InternalServerError);
                    }
                }
                catch (FileNotFoundException)
                {
                    responseObject.Content = GenerateErrorPage(HttpStatusCode.NotFound);
                }
                catch (Exception)
                {
                    responseObject.Content = GenerateErrorPage(HttpStatusCode.InternalServerError);
                }
            }
            byte[] buffer = new byte[responseObject.Content.Length];
            int nbytes;
            
            HttpResponse.ContentType = responseObject.ContentType;
            HttpResponse.ContentLength64 = buffer.Length;
            while ((nbytes = responseObject.Content.Read(buffer, 0, buffer.Length)) > 0)
                 HttpResponse.OutputStream.Write(buffer, 0, nbytes);
           // HttpResponse.OutputStream.Write(buffer, 0, buffer.Length);
            HttpResponse.StatusCode = (int)HttpStatusCode.OK;
            HttpResponse.OutputStream.Flush();

            HttpResponse.OutputStream.Close();
            responseObject.Content.Close();
        }






        public ResponseObject InvokeMethod(Uri url, string httpMethod, Stream httpInputStream)
        {
            //Removes the "/api" substring
            string methodName = url.AbsolutePath.Remove(0, 4);
            string[] strParams = url.Segments.Skip(2).Select(s => s.Replace("/", "")).ToArray();

            string body = "";

            var methodToInvoke = this.GetType().GetMethods()
                .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is RestRoute && ((RestRoute)attr).Route == methodName && ((RestRoute)attr).HttpMethod == httpMethod))
                .First();
            using (StreamReader sr = new StreamReader(httpInputStream))
            {
                body = sr.ReadToEnd();
                sr.Close();
                httpInputStream.Close();
            }
            object[] parameters = methodToInvoke.GetParameters().Select((p, i) => Convert.ChangeType(body, p.ParameterType)).ToArray();

            object ret = methodToInvoke.Invoke(this, parameters);

            return new ResponseObject(methodName, Router.GenerateStreamFromString(JsonConvert.SerializeObject(ret)), "application/json");

        }


        public ResponseObject InvokeMethod(Uri url, string httpMethod)
        {
            //Removes the "/api" substring
            string methodName = url.AbsolutePath.Remove(0, 4);
            if (httpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                int indexToTrimEnd = methodName.LastIndexOf('/');
                methodName = methodName.Remove(indexToTrimEnd, methodName.Length - indexToTrimEnd);
            }
            string[] strParams = url.Segments.Skip(4).Select(s => s.Replace("/", "")).ToArray();

            var methodToInvoke = this.GetType().GetMethods()
                .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is RestRoute && ((RestRoute)attr).Route == methodName && ((RestRoute)attr).HttpMethod == httpMethod))
                .First();

            object[] parameters = methodToInvoke.GetParameters().Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType)).ToArray();

            object ret = methodToInvoke.Invoke(this, parameters);

            return new ResponseObject(methodName, Router.GenerateStreamFromString(JsonConvert.SerializeObject(ret)), "application/json");

        }


        //Get RSA key
        [RestRoute("/get_key", "GET")]
        public string GetKey()
        {
            return cryption.GetRemKey();
        }

        [RestRoute("/braintree_token", "GET")]
        public string BraintreeToken()
        {
            return BraintreeClient.gateway.ClientToken.Generate();
        }

        [RestRoute("/login", "POST")]
        public string UserLogin(string json)
        {
            int? id = UserManager.VerifyUser(json);
            if (id.HasValue)
            {
                return Token.GenerateNew(id.Value);
            }
            return "Bad credentials!";

        }

        [RestRoute("/signup", "POST")]
        public User AddUser(string json)
        {
            return UserManager.AddUser(json);
        }


        [RestRoute("/getNewestOrders", "GET")]
        public Order GetOrder()
        {
            return OrderManager.GetOrder();
        }


        [RestRoute("/getMenuData", "GET")]
        public TreeviewObject GetMenuData()
        {
            TreeviewObject treeview = new TreeviewObject();

            treeview.Categories = CategoryManager.GetCategories();
            treeview.Food = FoodManager.GetFood();
            return treeview;
        }

        [RestRoute("/category/add", "POST")]
        public Category AddCategory(string jsonObject)
        {
            return CategoryManager.AddCategory(jsonObject);
        }

        [RestRoute("/category/delete", "DELETE")]
        public void RemoveCategory(int id)
        {
            // var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject);
            CategoryManager.DeleteCategory(id);
        }

        [RestRoute("/order/add", "POST")]
        public Order AddOrder(string json)
        {
            var order = OrderManager.AddOrder(json);
            WebServer.waitHandle.Set();
            return order;
        }

        [RestRoute("/food/add", "POST")]
        public Food AddFood(string jsonObject)
        {
            return FoodManager.AddFood(jsonObject);
        }

        [RestRoute("/food/update", "PUT")]
        public Food UpdateFood(string jsonObject)
        {
            return FoodManager.UpdateFood(jsonObject);
        }

        [RestRoute("/food/delete", "DELETE")]
        public void RemoveFood(int id)
        {
            FoodManager.DeleteFood(id);
        }






    }


}
