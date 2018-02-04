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

        public static Stream ReadBinaryFile(string name)
        {
            string filename = SanitizeFilename(name);

            FileStream fs = null;

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(message: "File was not found");
            }

            try
            {
                fs = File.Open(filename, FileMode.Open);
            }
            catch (IOException e)
            {
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
            if (Path.GetExtension(name) == ".ico")
            {
                return new ResponseObject(name, ReadBinaryFile(name));
            }
            StreamReader sr = ReadFile(name);
            //If requested file is not a html, it's returned directly
            //If it is, then it is proccessed and rendered by Templating engine
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
