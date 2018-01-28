using System;
using System.Collections.Generic;
using System.IO;
using WebServer.Http;

namespace WebServer.TempEngine
{
    class TempEngine
    {
        public static List<Keyword> FindKeywords(string rawHtml) {
            int start = rawHtml.IndexOf("{{");
            var keywords = new List<Keyword>();
            if (start == -1)
                return null;
            int end = rawHtml.IndexOf("}}");
            int length;
            
            keywords.Add(new Keyword(start, end, rawHtml.Substring(start, (end - start) + 2)));
            while(start < rawHtml.Length) {
                start = rawHtml.IndexOf("{{", start + 2);
                end = rawHtml.IndexOf("}}", end + 3);
                length = (end - start);
                if (length > 0)
                    keywords.Add(new Keyword(start, end, rawHtml.Substring(start, length + 2)));
                else
                    break;
            }
            return keywords;

        }


        // I have to bug fix multiple Imports
        public static string FindSection(string sectionName, string html)
        {
            string section = "{{ section start " + sectionName + " }}";
            int start = html.IndexOf(section, StringComparison.OrdinalIgnoreCase) + section.Length;
            int end = 0;
            if (start != -1)
            {
                end = html.IndexOf("{{ section end }}", start, StringComparison.OrdinalIgnoreCase);
                if (end == -1)
                    throw new SectionNotFoundException(string.Format("Section {0} ending was not found.", sectionName));
            }
            else
            {
                throw new SectionNotFoundException(string.Format("Section {0} was not found.", sectionName));

            }
            return html.Substring(start, end - start);
        }

        private static string AnalyzeKeywords(Keyword[] keywords, string html) {
            try
            {
                foreach (Keyword k in keywords)
                {
                    k.Analyze();
                    if (k.Type == Keyword.Types.Import) {
                        html = html.Insert(k.EndIndex, k.Import());
                    }
                }
            }
            catch (IncorrectTemplateSyntaxException ex)
            {
                throw ex;
            }
            return html;
        }

        private static string StripKeywords(string rawHtml, Keyword[] keywords) {
            //Because deleting from string shortens it, every keyword begin index then is kinda useless
            //I should've probably delete them right as I am trying to find them, but right now
            //I will just use a variable to determine the number I have to subtract to get the right index
            int offIndex = 0;
            foreach (Keyword k in keywords) {
                rawHtml = rawHtml.Remove(k.BeginIndex - offIndex, k.Length);
                offIndex += k.Length;
            }

            return rawHtml;
        }

        public static ResponseObject Render(StreamReader html)
        {
            string rawHtml = html.ReadToEnd();
            string processedHtml = "";
            var keywords = FindKeywords(rawHtml).ToArray();
            try
            {
                rawHtml = AnalyzeKeywords(keywords, rawHtml);
                processedHtml = StripKeywords(rawHtml, keywords);
            }
            catch (TemplateSyntaxException ex)
            {
                return new ResponseObject("error.html", Router.GenerateErrorPage(System.Net.HttpStatusCode.InternalServerError, ex.Message));
            }
            return new ResponseObject("success.html", Router.GenerateStreamFromString(processedHtml) );
        }


    }
}
