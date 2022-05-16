namespace JukaCompiler.Interpreter
{
    internal interface IJukaCallable
    {
        int Arity();
        Object Call(JukaInterpreter interpreter, List<Object> arguments);
    }
}
