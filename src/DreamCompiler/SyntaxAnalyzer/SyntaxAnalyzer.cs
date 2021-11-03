using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamCompiler.Constants;
using DreamCompiler.Scan;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DreamCompiler.SyntaxAnalyzer
{
    public class SyntaxAnalyzer
    {
        private LexemeListManager list;
        private LexemeEnumerator lexemeEnumerator;

        public void Analyze(LexemeListManager listManager)
        {
            list = listManager;

            lexemeEnumerator = listManager.GetEnumerator();
            
            if (lexemeEnumerator.MoveNextEx())
            {
                KeyWords(lexemeEnumerator.Current);
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
            while (lexemeEnumerator.MoveNextEx())
            {
                var current = lexemeEnumerator.Current;
                if (current.IsKeyWord() && current.KeyWordType == Lexer.KeyWords.KeyWordsEnum.If)
                {
                    lexemeEnumerator.MoveNextEx();
                    List<Lexeme> ifExpression = new List<Lexeme>();

                    if (lexemeEnumerator.Current.ToString().Equals("("))
                    {
                        lexemeEnumerator.MoveNextEx();
                        while (!lexemeEnumerator.Current.ToString().Equals(")"))
                        {
                            ifExpression.Add(lexemeEnumerator.Current);
                            lexemeEnumerator.MoveNextEx();
                        }
                    }
                }
            }
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
            var token = lexemeEnumerator.MoveNextEx();

        }
    }
}
