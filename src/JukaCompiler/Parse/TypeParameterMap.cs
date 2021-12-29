using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Lexer;

namespace JukaCompiler.Parse
{
    internal class TypeParameterMap
    {
        internal TypeParameterMap(Lexeme parameterType, Expression varName)
        {
            this.parameterType = parameterType;
            this.parameterName = varName;
        }

        internal Lexeme parameterType;
        internal Expression parameterName;
    }
}
