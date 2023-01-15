using System.Text;
using JukaCompiler.Expressions;
using JukaCompiler.Interpreter;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace JukaCompiler.SystemCalls
{
    public enum CallableServices
    {
        GetAvailableMemory,
        FileOpen,
        CSharp
    }

    /// <summary>
    /// Need a way to implement structured exception handling
    /// All this does is catch an exception and returns an object.
    /// </summary>
    internal class SystemCallException : Exception
    {
        internal Exception internalException;
        internal SystemCallException(Exception ex)
        {
            internalException = ex;
        }
    }

    internal class JukaSystemCalls : IJukaCallable
    {
        public static readonly Dictionary<string, Type> kv = new()
        {
            {"FileOpen", typeof(IFileOpen)},
            {"CSharp", typeof(ICSharp)},
        };

        public int Arity()
        {
            throw new NotImplementedException();
        }

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            if(JukaSystemCalls.kv.TryGetValue(methodName, out var theType))
            { 
                var jukacall = (IJukaCallable)interpreter.ServiceProvider.GetService(theType)!;
                return jukacall?.Call(methodName, interpreter, arguments);
            }

            throw new Exception("");
        }
    }

    internal class FileOpen : IFileOpen, IJukaCallable
    {
        public int Arity()
        {
            return 1;
        }

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            try
            { 
                foreach(var argument in arguments)
                {
                    if (argument is Expr.LexemeTypeLiteral literal)
                    {
                        byte[] bytes = File.ReadAllBytes( literal.literal?.ToString() ?? string.Empty);
                        Console.Out.WriteLine(literal);
                        return bytes;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new SystemCallException(ex);
            }

            return Array.Empty<byte>();
        }
    }

    internal class CSharp : ICSharp, IJukaCallable
    {
        public int Arity()
        {
            return 1;
        }

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            try
            {
                foreach (var argument in arguments)
                {
                    if (argument is Expr.LexemeTypeLiteral literal)
                    {
                        var csharp = literal.literal?.ToString() ?? string.Empty;

                        string result = "";
                        try
                        {
                            var task = CSharpScript.EvaluateAsync(csharp);
                            var taskCompleted = task.GetAwaiter();
                            if (taskCompleted.GetResult() != null)
                            {
                                result = taskCompleted.GetResult().ToString() ?? "";
                            }
                        }
                        catch (Exception)
                        {
                            result = string.Empty;
                        }

                        return  result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SystemCallException(ex);
            }

            return "";
        }
    }
}
