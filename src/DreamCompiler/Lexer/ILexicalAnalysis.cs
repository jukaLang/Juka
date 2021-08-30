using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamCompiler.Scan;

namespace DreamCompiler.Lexer
{
    public interface ILexicalAnalysis
    {
        LexemeListManager Analyze(IScanner scanner);
    }
}
