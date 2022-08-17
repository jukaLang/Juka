using JukaCompiler.Parse;

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
                        if (variable.ExpressionLexeme.ToString().Equals(expr.ExpressionLexeme.ToString(), StringComparison.OrdinalIgnoreCase))
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
