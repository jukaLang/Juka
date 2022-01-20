namespace JukaCompiler.Exceptions
{
    internal interface ICompilerError
    {
        internal void SourceFileName(string fileName);
        internal void AddError(string errorMessage);
        internal bool HasErrors();
        internal List<String> ListErrors();
    }
}
