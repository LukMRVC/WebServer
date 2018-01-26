using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Http.TempEngine
{
    public class IncorrectTemplateSyntaxException : Exception
    {
        public IncorrectTemplateSyntaxException()
        {
        }

        public IncorrectTemplateSyntaxException(string message)
            : base(message)
        {
        }

    }
}
