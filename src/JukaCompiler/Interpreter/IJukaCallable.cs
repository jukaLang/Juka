namespace JukaCompiler.Interpreter
{
    internal interface IJukaCallable
    {
        int Arity();
        object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments);
    }
}
