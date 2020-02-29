using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamCompiler.Scanner;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DreamCompiler.SyntaxAnalyzer
{
    public class SyntaxAnalyzer
    {
        public void Analyze(List<Lexeme> lexemeList)
        {
            foreach (var lex in lexemeList)
            {
                if (lex.IsKeyWord())
                {

                }
            }
        }
    }
}
