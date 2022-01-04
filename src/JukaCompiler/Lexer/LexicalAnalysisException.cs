namespace JukaCompiler.Lexer
{
    public class LexicalAnalysisException : Exception
    {
        public LexicalAnalysisException()
            : base()
        {
        }

        public LexicalAnalysisException(string message)
            : base(message)
        {
        }
    }
}
