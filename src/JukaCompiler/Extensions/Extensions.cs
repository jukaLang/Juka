using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Parse;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JukaCompiler.Extensions
{
    static internal class Extensions
    {
        public static bool TryGetValueEx<T,Y>(this Dictionary<T,Y> dict, Expression expr, out int? distance)
        {
            distance = null;
            foreach(var kvp in dict)
            {
                if (kvp.Key is Expression && kvp.Key is Expression.Variable)
                {
                    var variable = kvp.Key as Parse.Expression.Variable;
                    if (variable != null)
                    {
                        if (variable.Name.ToString().Equals(expr.Name.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            distance = (int)(object)kvp.Value;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
