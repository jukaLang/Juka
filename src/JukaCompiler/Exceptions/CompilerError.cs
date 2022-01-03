namespace JukaCompiler.Exceptions
{
    internal class CompilerError : ICompilerError
    {
        internal List<String> Errors = new List<String> ();

        public CompilerError()
        {
        }
        void ICompilerError.AddError(string errorMessage)
        {
            Errors.Add(errorMessage);
            System.Diagnostics.Debugger.Break();
        }

        bool ICompilerError.HasErrors()
        {
            return Errors.Count > 0;
        }

        List<String> ICompilerError.ListErrors()
        {
            return Errors;
        }
    }
}
