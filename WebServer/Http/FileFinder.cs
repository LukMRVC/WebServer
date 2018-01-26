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

        public static ResponseObject ReadFile(string name)
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
            if(Path.GetExtension(filename) == ".html")
                return TempEngine.TempEngine.Render(fs);
            return new ResponseObject(filename, new MemoryStream(Encoding.UTF8.GetBytes(fs.ReadToEnd())));

        }
    }
}
