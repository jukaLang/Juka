namespace JukaCompiler.Interpreter
{
    internal interface Callable
    {
        int Arity();
        Object Call(JukaInterpreter interpreter, List<Object> arguments);
    }
}
