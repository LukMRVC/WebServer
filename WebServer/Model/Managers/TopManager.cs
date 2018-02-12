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
            string[] strParams = url.Segments.Skip(2).Select(s => s.Replace("/", "")).ToArray();

            var methodToInvoke = this.GetType().GetMethods()
                .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is RestRoute && ((RestRoute)attr).Route == methodName && ((RestRoute)attr).HttpMethod == httpMethod))
                .First();

            object[] parameters = methodToInvoke.GetParameters().Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType)).ToArray();

            object ret = methodToInvoke.Invoke(this, parameters);

            return new ResponseObject(methodName, Router.GenerateStreamFromString(JsonConvert.SerializeObject(ret)), "application/json");

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
            // var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject);
            return CategoryManager.AddCategory(jsonObject);
        }

        [RestRoute("/food/add", "POST")]
        public Food AddFood(string jsonObject)
        {
            return FoodManager.AddFood(jsonObject);
        }

    }
}
