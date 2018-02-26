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

        private static HttpListenerRequest ListenerRequest;

        public static int StatusCode = 400;

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
            ListenerRequest = request;
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
                        StatusCode = 500;
                        responseObject.Content = GenerateErrorPage(HttpStatusCode.InternalServerError);
                    }
                }
                catch (FileNotFoundException)
                {
                    responseObject.Content = GenerateErrorPage(HttpStatusCode.NotFound);
                    StatusCode = 404;
                }
                catch (Exception)
                {
                    StatusCode = 500;
                    responseObject.Content = GenerateErrorPage(HttpStatusCode.InternalServerError);
                }
            }
            byte[] buffer = new byte[responseObject.Content.Length];
            int nbytes;

            HttpResponse.StatusCode = StatusCode;
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
            StatusCode = 200;
            return cryption.GetRemKey();
        }

        [RestRoute("/braintree_token", "GET")]
        public string BraintreeToken()
        {
            StatusCode = 200;
            return BraintreeClient.gateway.ClientToken.Generate();
        }

        [RestRoute("/get_user_history", "GET")]
        public ICollection<Order> GetOrderHistory()
        {
            int? id = Token.Verify(ListenerRequest.Headers.Get("Authorization"));
            if (id.HasValue)
            {
                StatusCode = 200;
                return OrderManager.GetHistory(id.Value);
            }
            StatusCode = 404;
            return new List<Order>();
        }

        [RestRoute("/login", "POST")]
        public string UserLogin(string json)
        {
            int? id = UserManager.VerifyUser(json);
            if (id.HasValue)
            {
                StatusCode = 200;
                return Token.GenerateNew(id.Value);
            }
            StatusCode = 403;
            return "Bad credentials!";

        }

        [RestRoute("/login", "GET")]
        public string UserTokenLogin()
        {
            int? id = Token.Verify(ListenerRequest.Headers.Get("Authorization"));
            if (id.HasValue)
            {
                StatusCode = 200;
                return Token.GenerateNew(id.Value);
            }
            StatusCode = 403;
            return "Invalid Token";
        }

        [RestRoute("/order", "POST")]
        public Order AddOrder(string json)
        {
            Order order;
            int? id = Token.Verify(ListenerRequest.Headers.Get("Authorization"));
            if (id.HasValue)
            {
                WebServer.waitHandle.Set();
                order = OrderManager.AddOrder(json, id.Value);
                StatusCode = 201;
                return order;
            }
            StatusCode = 403;
            return null;
        }

        [RestRoute("/pay", "POST")]
        public string Pay(string json)
        {
            int? id = Token.Verify(ListenerRequest.Headers.Get("Authorization"));
            if (id.HasValue)
            {
                var payment = JsonConvert.DeserializeObject<BraintreeClient.PaymentRequest>(json);
                var request = new Braintree.TransactionRequest
                {
                    Amount = payment.Amount,
                    MerchantAccountId = "Sandbox_Project",
                    PaymentMethodNonce = payment.Nonce,
                    CustomerId = id.Value.ToString(),
                    Options = new Braintree.TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Braintree.Result<Braintree.Transaction> result = BraintreeClient.gateway.Transaction.Sale(request);
                if (result.IsSuccess())
                {
                    StatusCode = 200;
                    return "Successfully paid.";
                }
                else
                {
                    StatusCode = 400;
                    return "Error while paying";
                }

            }
            else {
                StatusCode = 403;
                return "Invalid token/user ID";
            }
            
        }

        [RestRoute("/signup", "POST")]
        public User AddUser(string json)
        {
            StatusCode = 201;
            return UserManager.AddUser(json);
        }


        [RestRoute("/getNewestOrders", "GET")]
        public Order GetOrder()
        {
            StatusCode = 200;
            return OrderManager.GetOrder();
        }


        [RestRoute("/getMenuData", "GET")]
        public TreeviewObject GetMenuData()
        {
            TreeviewObject treeview = new TreeviewObject();

            treeview.Categories = CategoryManager.GetCategories();
            treeview.Food = FoodManager.GetFood();
            StatusCode = 200;
            return treeview;
        }

        [RestRoute("/category/add", "POST")]
        public Category AddCategory(string jsonObject)
        {
            StatusCode = 201;
            return CategoryManager.AddCategory(jsonObject);
        }

        [RestRoute("/category/delete", "DELETE")]
        public void RemoveCategory(int id)
        {
            // var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject);
            CategoryManager.DeleteCategory(id);
            StatusCode = 204;
        }

        [RestRoute("/food/add", "POST")]
        public Food AddFood(string jsonObject)
        {
            StatusCode = 201;
            return FoodManager.AddFood(jsonObject);
        }

        [RestRoute("/food/update", "PUT")]
        public Food UpdateFood(string jsonObject)
        {
            StatusCode = 202;
            return FoodManager.UpdateFood(jsonObject);
        }

        [RestRoute("/food/delete", "DELETE")]
        public void RemoveFood(int id)
        {
            StatusCode = 204;
            FoodManager.DeleteFood(id);
        }






    }


}
