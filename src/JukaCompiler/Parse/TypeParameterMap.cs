using JukaCompiler.Expressions;
using JukaCompiler.Lexer;

namespace JukaCompiler.Parse
{
    internal class TypeParameterMap
    {
        internal TypeParameterMap(Lexeme? parameterType, Expr varName)
        {
            this.parameterType = parameterType;
            parameterName = varName;
        }

        internal Lexeme? parameterType;
        internal Expr parameterName;
    }
}
