namespace JukaCompiler.Interpreter
{
    internal interface IJukaCallable
    {
        int Arity();
        Object Call(string methodName, JukaInterpreter interpreter, List<Object> arguments);
    }
}
