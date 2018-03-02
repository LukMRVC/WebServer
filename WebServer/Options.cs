using System;
using System.Collections.Generic;
using System.Text;
using clipr;

namespace WebServer
{
    public class Options
    {

        [NamedArgument('m', "host", Action = ParseAction.Store,
            Const = "locahost", Description = "Database host",
            Required = false, Constraint = NumArgsConstraint.Exactly, NumArgs = 1)]
        public string Host { get; set; }


        [NamedArgument('d', "database", Action = ParseAction.Store,
            Const = "webserverdb", Description = "Database name",
            Required = false, Constraint = NumArgsConstraint.Exactly, NumArgs = 1 )]
        public string Database { get; set; }

        [NamedArgument('u', "user", Action = ParseAction.Store,
            Const = "root", Description = "Database user name",
            Required = false, Constraint = NumArgsConstraint.Exactly, NumArgs = 1)]
        public string User { get; set; }

        [PromptIfValueMissing(MaskInput = true)]
        [NamedArgument('s', "secret", Description = "Database account secret" ,Const = "" ,Required = false, Constraint = NumArgsConstraint.Exactly,
            NumArgs = 1, Action = ParseAction.Store)]
        public string Password { get; set; }

    }
}
