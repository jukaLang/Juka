using JukaCompiler.Exceptions;
using JukaCompiler.Expressions;
using JukaCompiler.Lexer;
using JukaCompiler.Statements;

namespace JukaCompiler.Interpreter
{
    internal class StackFrame
    {
        private Dictionary<string, Lexeme> frameVariables = new();
        private string frameName;
        private Dictionary<string, object?> variables = new();
        private Dictionary<string, StackVariableState> variableAndKind = new();

        internal StackFrame(string name)
        {
            this.frameName = name;
        }

        internal void AddVariables(Dictionary<string, object?> variables, JukaInterpreter interpreter)
        {
            foreach (var variable in variables)
            {
                string name = variable.Key;
                object? value = ((Expr.LexemeTypeLiteral) variable.Value!)?.literal;
                AddVariable(name, value, variable.Value?.GetType(), null);
            }
        }

        internal object? AddVariable(Stmt.Var variable, JukaInterpreter interpreter)
        {
            if (variable.exprInitializer != null)
            {
                object? variableValue = interpreter.Evaluate(variable.exprInitializer);

                if (variableValue == null)
                {
                    throw new JRuntimeException("the value of the variable is null");
                }

                if (!(variable.exprInitializer is Expr.Call))
                {
                    string variableName = variable.name?.ToString() ?? throw new JRuntimeException("variable name is missing");
                    AddVariable(variableName, variableValue, variableValue.GetType(),variable.exprInitializer);
                }

                return variableValue;
            }

            throw new JRuntimeException("unable to add variable");
        }

        internal void AddVariable(string name, object? variableValue, Type? variableKind, Expr? expressionContext)
        {
            var stackVariableState = new StackVariableState
            {
                Name = name,
                Value = variableValue,
                type = variableKind,
                expressionContext = expressionContext,
            };

            if (variableAndKind.ContainsKey(name))
            {
                variableAndKind[name] = stackVariableState;
            }
            else
            {
                variables.Add(name, stackVariableState);
            }
        }

        internal bool UpdateVariable(string name, object? value)
        {
            if (variables.ContainsKey(name))
            {
                variables[name] = value;
                return true;
            }

            return false;
        }

        internal bool DeleteVariable(string name)
        {
            if (variables.ContainsKey(name))
            {
                variables.Remove(name);
                return true;
            }

            return false;
        }

        internal bool TryGetStackVariableByName(string name, out StackVariableState? variable)
        {
            variable = null;

            if(variables.ContainsKey(name))
            {
                variable = (StackVariableState?) this.variables[name];
                return true;
            }

            return false;
        }

        internal class StackVariableState
        {
            internal string Name = null!;
            internal object? Value;
            internal Type? type;
            internal Expr? expressionContext;
            internal object[] arrayValues;
        }
    }
}
