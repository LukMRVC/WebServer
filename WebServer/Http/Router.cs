using System;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace WebServer.Http
{
    class Router
    {


        public static HttpListenerResponse response;
        public static Model.Managers.TopManager manager = new Model.Managers.TopManager();


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
            response = HttpResponse;
            ResponseObject responseObject = new ResponseObject();
            if (request.Url.AbsolutePath.Contains("submit"))
            {
                manager.InvokeMethod(request.Url, request.HttpMethod);
                response.Redirect(@"http://localhost:1234/");
            }
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
            finally {
                byte[] buffer = new byte[responseObject.Content.Length];
                int nbytes;
                response.ContentType = responseObject.ContentType;
                response.ContentLength64 = buffer.Length;
                while ((nbytes = responseObject.Content.Read(buffer, 0, buffer.Length)) > 0)
                    response.OutputStream.Write(buffer, 0, nbytes);
                responseObject.Content.Close();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.OutputStream.Flush();
                response.OutputStream.Close();
            }
        }
    }

    
}
