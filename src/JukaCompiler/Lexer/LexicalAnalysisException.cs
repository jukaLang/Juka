using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Lexer
{
    public class LexicalAnalysisException : Exception
    {
        public LexicalAnalysisException()
            : base()
        {
        }

        public LexicalAnalysisException(string message)
            : base(message)
        {
        }
    }
}
