using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamCompiler.Constants;
using DreamCompiler.Scanner;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DreamCompiler.SyntaxAnalyzer
{
    public class SyntaxAnalyzer
    {
        private LexemeListManager list;
        private int currentLocation = 0;
        public void Analyze(LexemeListManager listManager)
        {
            list = listManager;

            var listEnumerator = listManager.GetEnumerator();
            while (listEnumerator.MoveNext())
            {
                if (listEnumerator.Current.IsKeyWord())
                {
                    KeyWords(listEnumerator.Current);
                }
            }

            /*
            while(list.NextNotWhiteSpace())
            for(; currentLocation < list.Count; currentLocation++)
            {
                if (list[currentLocation].IsKeyWord())
                {
                    KeyWords(list[currentLocation]);
                }
            }
            */
        }

        private void KeyWords(Lexeme lex)
        {
            Trace.WriteLine(lex.ToString());
            switch (lex.KeyWordType)
            {
                case Lexer.KeyWords.KeyWordsEnum.Function:
                {
                    ParseFunction();
                    break;
                }


                case Lexer.KeyWords.KeyWordsEnum.If:
                {
                    ParseIfStatement();
                    break;
                }
            }
        }


        private void ParseFunction()
        {
            currentLocation++; // skip space;
            var funcName = list[this.currentLocation++];
            var leftParen = list[this.currentLocation++];
            var rightParen = list[this.currentLocation++];
            var equalSign = list[this.currentLocation++];
            var leftBracket = list[this.currentLocation++];
        }

        private void ParseIfStatement()
        {
            var token = list[++currentLocation];
            if (!token.IsKeyWord() && token.ToString().Equals(SymbolConstants.LeftParen))
            {
                ParseExpression();
            }
        }


        private void ParseExpression()
        {
            var token = list[++currentLocation];

        }
    }
}
