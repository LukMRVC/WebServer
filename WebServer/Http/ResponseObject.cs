using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebServer.Http
{
    public class ResponseObject
    {

        public ResponseObject(string filename, Stream stream)
        {
            Content = stream;
            ContentType = MimeMappings.TryGetValue(Path.GetExtension(filename), out string mime) ? mime : "application/octet-stream";
        }

        public ResponseObject(string filename, Stream stream, string contentType)
        {
            Content = stream;
            ContentType = contentType;
        }

        public ResponseObject()
        {
            ContentType = "text/html";

        }

        private static Dictionary<string, string> MimeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".html", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/x-javascript" },
            { ".ico", "image/x-icon" }
        };

        public string ContentType { get; set; }

        public Stream Content { get; set; }

    }
}
