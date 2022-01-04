namespace JukaCompiler.Exceptions
{
    internal interface ICompilerError
    {
        internal void AddError(string errorMessage);
        internal bool HasErrors();
        internal List<String> ListErrors();
    }
}
