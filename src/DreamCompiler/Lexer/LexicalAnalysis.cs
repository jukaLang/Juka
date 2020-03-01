using DreamCompiler.Scanner;
using DreamCompiler.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace DreamCompiler.Lexer
{
    public class LexicalAnalysis
    {
        private Scanner.Scanner scanner;
        public Dictionary<KeyWords.KeyWordsEnum, Action<IToken>> keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>();
        
        
        public LexicalAnalysis(Scanner.Scanner scanner)
        {
            this.scanner = scanner;

            keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>()
            {
                {KeyWords.KeyWordsEnum.Main, MainAction},
                {KeyWords.KeyWordsEnum.Function, MainAction },
            };
        }

        public LexemeListManager Analyze()
        {
            List<Lexeme> lexemeList = new List<Lexeme>();
            while (true)
            {
                IToken token = scanner.ReadToken();
                if (token.TokenType() == TokenType.Eof)
                {
                    break;
                }

                switch (token.TokenType())
                {
                    case TokenType.Character:
                    {
                        lexemeList.Add(GetIdentifier(token));
                        break;
                    }

                    case TokenType.NumberDigit:
                    {
                        lexemeList.Add(GetNumber(token));
                        break;
                    }

                    case TokenType.Symbol:
                    {
                        lexemeList.Add(GetSymbol(token));
                        break;
                    }

                    case TokenType.WhiteSpace:
                    {
                        lexemeList.Add(GetWhiteSpace(token));
                        break;
                    }
                }
            }

            return new LexemeListManager(lexemeList);
        }

        //public void 


        private Lexeme GetIdentifier(IToken token)
        {
            if (token.TokenType() == TokenType.WhiteSpace)
            {
                while (token.TokenType() == TokenType.WhiteSpace)
                {
                    token = scanner.ReadToken();
                }
            }

            using (Lexeme identifier = new Lexeme(LexemeType.Identifier))
            {
                identifier.AddToken(token);

                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.TokenType() == TokenType.Symbol || next.TokenType() == TokenType.WhiteSpace)
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }

                    identifier.AddToken(next);
                }

                identifier.PrintLexeme("Identifier");

                return identifier;
            }

            throw new Exception();
        }


        private Lexeme GetNumber(IToken token)
        {
            Lexeme number = new Lexeme(LexemeType.Number);
            number.AddToken(token);

            while(true)
            {
                var next = this.scanner.ReadToken();
                if (next.TokenType() == TokenType.NumberDigit)
                {
                    number.AddToken(token);
                }
                else
                {
                    this.scanner.PutTokenBack();
                    break;
                }
            }

            number.PrintLexeme("Number");
            return number;
        }

        private IToken GetPunctuation(IToken token)
        {
            while (token.TokenType() == TokenType.WhiteSpace)
            {
                token = scanner.ReadToken();
            }

            if (token.TokenType() == TokenType.Symbol)
            {
                return token;
            }

            throw new Exception("No Punctuation found;");
        }

        private Lexeme GetSymbol(IToken token)
        {
            Lexeme symbol = new Lexeme(LexemeType.Number);

            var currentSymbol = token.GetTokenData();
            if (currentSymbol == '(' ||
                currentSymbol == ')' ||
                currentSymbol == '"' ||
                currentSymbol == '{' ||
                currentSymbol == '}' ||
                currentSymbol == ';'
                )
            {
                symbol.AddToken(token);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }


            if (currentSymbol == '=')
            {
                symbol.AddToken(token);
                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.GetTokenData() == '=')
                    {
                        symbol.AddToken(next);
                    }
                    else
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }
                }
            }
            else if (currentSymbol == '<')
            {
                symbol.AddToken(token);
                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.GetTokenData() == '=')
                    {
                        symbol.AddToken(next);
                    }
                    else
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }
                }
            }

            symbol.PrintLexeme("Symbol");
            return symbol;
        }

        private Lexeme GetWhiteSpace(IToken token)
        {
            Lexeme whiteSpace = new Lexeme(LexemeType.WhiteSpace);

            if (token.GetTokenData().Equals('\n') ||
                token.GetTokenData().Equals('\r') ||
                token.GetTokenData().Equals('\t') ||
                token.GetTokenData().Equals(' '))
            {
                whiteSpace.AddToken(token);
                whiteSpace.PrintLexeme("WhiteSpace", ()=>
                {
                    if (token.GetTokenData().Equals('\n'))
                    {
                        Trace.Write("\\n");
                    }
                    else if (token.GetTokenData().Equals('\r'))
                    {
                        Trace.Write("\\r");
                    }
                    else if (token.GetTokenData().Equals('\t'))
                    {
                        Trace.Write("\\t");
                    }
                    else if (token.GetTokenData().Equals(' '))
                    {
                        Trace.Write("_");
                    }
                });
            }

            return whiteSpace;
        }

        private void VisitIdentifier(IToken token)
        {
            Lexeme tokenIdentifier = GetIdentifier(token);

            /*
            if (KeyWords.keyValuePairs.TryGetValue(tokenIdentifier.ToString(), out KeyWords.KeyWordType keyWordsEnum))
            {
                if (keywordActions.ContainsKey(keyWordsEnum))
                {
                    keywordActions[keyWordsEnum](tokenIdentifier);
                }
            }
            */
        }

        private void MainAction(IToken token)
        {
            if (token.TokenType() != TokenType.Character)
            {
                throw new Exception("no function name");
            }

            if (KeyWords.keyWordNames.Contains(token.ToString()))
            {
                if (token.ToString().Equals(KeyWords.FUNCTION))
                {

                    Lexeme functionName = GetIdentifier(scanner.ReadToken());
                    IToken leftParen = GetPunctuation(scanner.ReadToken());

                    if (!leftParen.ToString().Equals(KeyWords.LPAREN))
                    {
                        throw new Exception("no left paren");
                    }

                    // TODO handle function parameters;

                    IToken rightParen = GetPunctuation(scanner.ReadToken());

                    if (!rightParen.ToString().Equals(KeyWords.RPAREN))
                    {
                        throw new Exception();
                    }

                    Lexeme isEqualSign = GetSymbol(scanner.ReadToken());
                    IToken isLeftBracket = GetPunctuation(scanner.ReadToken());
                }
            }
        }
    };
}


