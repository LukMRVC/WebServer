using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebServer.Http
{
    class FileFinder
    {
        private static string root = @"www/";

        public FileFinder() { }

        private static string SanitizeFilename(string filename)
        {
            if (filename.Contains("\'"))
            {
                filename = filename.Trim('\'');
            }

            string relative = "html/";
            if (filename.EndsWith('/')) {
                relative += filename;
                filename = "";
            }
            if (string.IsNullOrEmpty(filename))
            {
                filename = "index";
            }

            if (!Path.HasExtension(filename))
            {
                filename = relative + filename + ".html";
            }

            filename = Path.Combine(root, filename);

            return filename;
        }

        public static StreamReader ReadFile(string name)
        {
            string filename = SanitizeFilename(name);
            StreamReader fs = null;
            

            if (!File.Exists(filename)) {
                throw new FileNotFoundException(message: "File was not found");
            }

            try
            {
                fs = File.OpenText(filename);
            }
            catch (IOException e)
            {
                System.Console.WriteLine(e.ToString());
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }

            return fs;

        }

        public static ResponseObject GetFile(string name)
        {
            StreamReader sr = ReadFile(name);
            if (Path.HasExtension(name))
            {
                return new ResponseObject(name, new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd())));
            }
            else
            {
                return TempEngine.TempEngine.Render(sr);
            }



        }
    }
}
