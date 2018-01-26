using System;
using System.Collections.Generic;
using System.IO;

namespace WebServer.Http.TempEngine
{
    class TempEngine
    {

        private static List<Keyword> keywords;

        public static class Keywords
        {
            public static string Section { get { return "section"; } }
            public static string Start { get { return "start"; } }
            public static string End { get { return "end"; } }
            public static string Import { get { return "import"; } }

        }

        public class Keyword
        {
            int beginIndex;
            int endIndex;
            string word;

            public Keyword(int start, int end, string word) {
                beginIndex = start;
                endIndex = end;
                this.word = word;
            }

            public void Analyze()  {
                var arr = word.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length != 5)
                {
                    if(arr[1] != Keywords.Section && arr[2] != Keywords.End)
                    {
                        throw new IncorrectTemplateSyntaxException("Incorrect template syntax near: " + word);
                    }
                }
            }

        }

        public static void FindKeywords(string rawHtml) {
            int start = rawHtml.IndexOf("{{");
            if (start == -1)
                return;
            int end = rawHtml.IndexOf("}}");
            int length;
            keywords = new List<Keyword>();
            keywords.Add(new Keyword(start, end, rawHtml.Substring(start, (end - start) + 2)));
            while(start < rawHtml.Length) {
                start = rawHtml.IndexOf("{{", start + 2);
                end = rawHtml.IndexOf("}}", end + 3);
                length = (end - start);
                if (length > 0)
                    keywords.Add(new Keyword(start, end, rawHtml.Substring(start, length + 2)));
                else
                    break;
               /* if (rawHtml.Contains("{{") && rawHtml.Contains("}}")) {
                    start = rawHtml.IndexOf("{{", 0);
                    end = rawHtml.IndexOf("}}", start);
                    keywords.Add(rawHtml.Substring(start, end - start));
                }*/
            }

        }

        private static void AnalyzeKeywords() {
            foreach(Keyword k in keywords)
            {
                try
                {
                    k.Analyze();
                }
                catch (IncorrectTemplateSyntaxException ex) {
                    throw ex;
                }

            }
        }



        public static ResponseObject Render(StreamReader html)
        {
            string rawHtml = html.ReadToEnd();

            FindKeywords(rawHtml);
            try
            {
                AnalyzeKeywords();
            }
            catch (IncorrectTemplateSyntaxException ex)
            {
                return new ResponseObject("error.html", Router.GenerateErrorPage(System.Net.HttpStatusCode.InternalServerError, ex.Message));
            }
            return new ResponseObject("success.html", Http.Router.GenerateStreamFromString(rawHtml) );
        }


    }
}
