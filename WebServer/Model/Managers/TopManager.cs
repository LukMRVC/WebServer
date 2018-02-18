using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using WebServer.Http.REST.Attributes;
using WebServer.Http;

namespace WebServer.Model.Managers
{
    class TopManager
    {

        public TopManager()
        {

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
            if(httpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
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
            WebServer.Http.WebServer.waitHandle.Set();

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
