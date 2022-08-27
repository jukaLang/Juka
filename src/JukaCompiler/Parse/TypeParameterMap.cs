using JukaCompiler.Lexer;

namespace JukaCompiler.Parse
{
    internal class TypeParameterMap
    {
        internal TypeParameterMap(Lexeme? parameterType, Expression varName)
        {
            this.parameterType = parameterType;
            this.parameterName = varName;
        }

        internal Lexeme? parameterType;
        internal Expression parameterName;
    }
}
