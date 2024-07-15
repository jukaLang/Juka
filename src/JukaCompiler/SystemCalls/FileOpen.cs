using JukaCompiler.Expressions;
using JukaCompiler.Interpreter;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace JukaCompiler.SystemCalls
{
    public enum CallableServices
    {
        GetAvailableMemory,
        FileOpen,
        CSharp
    }

    /// <summary>
    /// Exception class for handling system call errors.
    /// </summary>
    internal class SystemCallException : Exception
    {
        internal Exception InternalException { get; }

        internal SystemCallException(Exception ex)
        {
            InternalException = ex;
        }
    }

    internal class JukaSystemCalls : IJukaCallable
    {
        private static Dictionary<string, Type> _callableTypes = new()
    {
        {"FileOpen", typeof(IFileOpener)},
        {"CSharp", typeof(ICSharp)},
    };

        public int Arity() => throw new NotImplementedException();

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            if (_callableTypes.TryGetValue(methodName, out Type? callableType))
            {
                IJukaCallable? jukaCallable = interpreter.ServiceProvider.GetService(callableType) as IJukaCallable;
                return jukaCallable?.Call(methodName, interpreter, arguments);
            }

            throw new Exception($"Method {methodName} not found.");
        }
    }

    internal class FileOpen : IFileOpener, IJukaCallable
    {
        public int Arity() => 1;

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            try
            {
                foreach (object? argument in arguments)
                {
                    if (argument is Expr.LexemeTypeLiteral literal)
                    {
                        string filePath = literal.literal?.ToString() ?? string.Empty;
                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        Console.Out.WriteLine(literal);
                        return fileBytes;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SystemCallException(ex);
            }

            return Array.Empty<byte>();
        }
    }

    internal class CSharp : ICSharp, IJukaCallable
    {
        public int Arity() => 1;

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            try
            {
                foreach (object? argument in arguments)
                {
                    if (argument is Expr.LexemeTypeLiteral literal)
                    {
                        string csharpCode = literal.literal?.ToString() ?? string.Empty;

                        string result = "";
                        try
                        {
                            Task<object> evaluationTask = CSharpScript.EvaluateAsync(csharpCode, ScriptOptions.Default.WithImports("System.IO", "System.Math"));
                            object taskResult = evaluationTask.GetAwaiter().GetResult();
                            result = taskResult?.ToString() ?? "";
                        }
                        catch (Exception)
                        {
                            result = string.Empty;
                        }

                        return result;
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
