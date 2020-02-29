using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamCompiler.Error
{
    using System.IO;
    using Antlr4.Runtime;
    internal class ErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}
