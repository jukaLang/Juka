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
        private LexemeEnumerator lexemeEnumerator;

        public void Analyze(LexemeListManager listManager)
        {
            list = listManager;

            lexemeEnumerator = listManager.GetEnumerator();
            var value = lexemeEnumerator.MoveNextSkipWhite();
            if (value.IsKeyWord())
            {
                KeyWords(value);
            }
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
            var funcName = lexemeEnumerator.MoveNextSkipWhite();
            var leftParen = lexemeEnumerator.MoveNextSkipWhite();
            var rightParen = lexemeEnumerator.MoveNextSkipWhite();
            var equalSign = lexemeEnumerator.MoveNextSkipWhite();
            var leftBracket = lexemeEnumerator.MoveNextSkipWhite();
        }

        private void ParseIfStatement()
        {
            /*
            if (lexemeEnumerator.MoveNextSkipWhite())
            {
                var token = lexemeEnumerator.Current;
                if (!token.IsKeyWord() && token.ToString().Equals(SymbolConstants.LeftParen))
                {
                    ParseExpression();
                }
            }
            */
        }


        private void ParseExpression()
        {
            var token = lexemeEnumerator.MoveNextSkipWhite();

        }
    }
}
