namespace JukaCompiler.Exceptions
{
    internal class JRuntimeException : Exception
    {
        internal JRuntimeException(string message)
            : base(message)
        {
        }
    }

    internal class JParserError : Exception
    {
        internal JParserError(string message)
            : base(message)
        {
        }
    }
}
