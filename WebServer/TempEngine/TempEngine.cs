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
            //if keyword was not found, return empty list
            if (start == -1)
                return keywords;
            int end = rawHtml.IndexOf("}}");
            int length;
            //between start and end should be the keyword, which I add
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


        //This could solve finding right section if they are nested
        private static int FindEndsection(string html, int from){
            int nested, ending;
            ending = html.IndexOf("{{ section end }}", from, StringComparison.OrdinalIgnoreCase);
            nested = html.IndexOf("{{ section start ", from, StringComparison.OrdinalIgnoreCase);
            //if section start is found earlier than ending, they are nested
            //So I skip it with recursive calling and passing ending as start index for next search of ending section
            if ((nested != -1 && ending != -1) && nested < ending)
            {
                ending = FindEndsection(html, ending);
            }
            return ending;
        }


        // I have to bug fix multiple Imports
        public static string FindSection(string sectionName, string html)
        {
            string section = "{{ section start " + sectionName + " }}";
            int start = html.IndexOf(section, StringComparison.OrdinalIgnoreCase) + section.Length;
            int end = 0;
            if (start != -1)
            {
                end = FindEndsection(html, start);
                if (end == -1)
                    throw new SectionNotFoundException(string.Format("Section {0} ending was not found.", sectionName));
            }
            else
            {
                throw new SectionNotFoundException(string.Format("Section {0} was not found.", sectionName));

            }
            return html.Substring(start, end - start);
        }



        private static string Parse(string html) {
            //Plus length is the length of added html string, that moves index of the begging of the keyword
            int plusLength = 0;
            var keywords = FindKeywords(html).ToArray();
            if(keywords.Length == 0)
            {
                return html;
            }
            string imported = "";
            try
            {
                foreach (Keyword k in keywords)
                {
                    k.Analyze();
                    if(plusLength > 0)
                        k.MoveIndices(plusLength);
                    if (k.Type == Keyword.Types.Import) {
                        imported = k.Import();
                        //Substract length to prevent IndexOutOfRangeException, because the keyword will be removed
                        plusLength += (imported.Length - k.Length);
                        html = html.Insert(k.EndIndex, imported);
                        html = html.Remove(k.BeginIndex, k.Length);
                    }
                }
                //If there were imported sections, parse html again
                if (plusLength > 0)
                {
                    html = Parse(html);
                }
            }
            catch (IncorrectTemplateSyntaxException ex)
            {
                throw ex;
            }
            return html;
        }

        private static string StripKeywords(string rawHtml) {
            //Because deleting from string shortens it, every keyword begin index then is kinda useless
            //I should've probably delete them right as I am trying to find them, but right now
            //I will just use a variable to determine the number I have to subtract to get the right index
            int offIndex = 0;
            var keywords = FindKeywords(rawHtml).ToArray();
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
            try
            {
                var templatedHtml = Parse(rawHtml);
                processedHtml = StripKeywords(templatedHtml);
            }
            catch (TemplateSyntaxException ex)
            {
                return new ResponseObject("error.html", Router.GenerateErrorPage(System.Net.HttpStatusCode.InternalServerError, ex.Message));
            }
            return new ResponseObject("success.html", Router.GenerateStreamFromString(processedHtml) );
        }


    }
}
