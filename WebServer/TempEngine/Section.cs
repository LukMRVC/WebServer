using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.TempEngine
{
    public class Section
    {

        public Section(int b, int e, string t)
        {
            Begin = b;
            End = e;
            Text = t;
        }


        public int Begin { get; private set; }
        public int End { get; private set; }
        public string Text { get; private set; }

    }
}
