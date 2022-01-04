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
