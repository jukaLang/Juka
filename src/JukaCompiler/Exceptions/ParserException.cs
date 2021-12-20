using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Lexer;

namespace JukaCompiler.Exceptions
{
    internal class ParserException : Exception
    {
        internal ParserException(string? message, LexemeType lexemeType) 
            : base(message)
        {
        }
    }
}
