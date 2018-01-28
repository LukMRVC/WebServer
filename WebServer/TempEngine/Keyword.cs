using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.TempEngine
{
    public static class Keywords
    {
        public static string Section { get { return "section"; } }
        public static string Start { get { return "start"; } }
        public static string End { get { return "end"; } }
        public static string Import { get { return "import"; } }

    }

    public class Keyword
    {
        public enum Types { Import, Section, Endsection };

        public int BeginIndex { get; private set; }
        public int EndIndex { get; private set; }
        public int Length { get; private set; }
        string word;
        public Types Type { get; private set; }
        private string[] Split;

        public Keyword(int start, int end, string word)
        {
            BeginIndex = start;
            EndIndex = (end + 2);
            Length = EndIndex - start;
            this.word = word;
        }

        public void Analyze()
        {
            var arr = word.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 5)
            {
                if (string.Equals(arr[1], Keywords.Section, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(arr[2], Keywords.Start, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Type = Types.Section;
                    }
                    
                    else
                    {
                        throw new IncorrectTemplateSyntaxException("Incorrect template syntax near: " + word);
                    }
                }
                else if (string.Equals(arr[1], Keywords.Import, StringComparison.OrdinalIgnoreCase))
                {
                    this.Type = Types.Import;
                }
                else
                {
                    throw new IncorrectTemplateSyntaxException("Incorrect template syntax near: " + word);
                }
            }
            else
            {
                if (string.Equals(arr[2], Keywords.End, StringComparison.OrdinalIgnoreCase))
                {
                    this.Type = Types.Endsection;
                }
                else {
                    throw new IncorrectTemplateSyntaxException("Incorrect template syntax near: " + word);
                }

            }
            this.Split = arr;
        }

        public string Import() {

            var stream = Http.FileFinder.ReadFile(Split[2]);
            string section = "";
            if (stream == null)
            {
                throw new IncorrectTemplateSyntaxException("Incorrect template import syntax near: " + word);
            }
            try
            {
                section = TempEngine.FindSection(Split[3], stream.ReadToEnd());
            }
            catch (SectionNotFoundException e) {
                throw e;
            }
            finally
            {
                stream.Close();
            }

            return section;
        }



    }
}
