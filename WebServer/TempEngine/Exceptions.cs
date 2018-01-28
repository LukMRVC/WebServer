using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.TempEngine
{
    public class TemplateSyntaxException : Exception
    {
        public TemplateSyntaxException()
        {
        }

        public TemplateSyntaxException(string message)
            : base(message)
        {
        }

    }


    public class IncorrectTemplateSyntaxException : TemplateSyntaxException
    {
        public IncorrectTemplateSyntaxException()
        {
        }

        public IncorrectTemplateSyntaxException(string message)
            : base(message)
        {
        }

    }

    public class SectionNotFoundException : TemplateSyntaxException
    {
        public SectionNotFoundException()
        {
        }

        public SectionNotFoundException(string message)
            : base(message)
        {
        }

    }
}
