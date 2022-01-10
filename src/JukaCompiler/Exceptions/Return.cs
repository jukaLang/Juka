namespace JukaCompiler.Exceptions
{
    internal class Return : JRuntimeException
    {
        internal object value;
        internal Return(object value)
            : base("return")
        {
            this.value = value; 
        }
    }
}
