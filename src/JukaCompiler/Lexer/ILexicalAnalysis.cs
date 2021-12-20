using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Scan;

namespace JukaCompiler.Lexer
{
    public interface ILexicalAnalysis
    {
        LexemeListManager Analyze(IScanner scanner);
    }
}
