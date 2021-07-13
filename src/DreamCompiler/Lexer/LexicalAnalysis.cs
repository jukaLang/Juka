using DreamCompiler.Scanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DreamCompiler.Lexer
{
    public class LexicalAnalysis : ILexicalAnalysis
    {
        private IScanner scanner;
        public Dictionary<KeyWords.KeyWordsEnum, Action<IToken>> keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>();
        private List<char> symbolList = new List<char>() {'=','/','-',',',';'};

    public LexicalAnalysis(IScanner scanner)
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
                if (token.TokenType() == TokenType.Eof || token.TokenType() == TokenType.NotValid)
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

        private Lexeme GetIdentifier(IToken token)
        {
            token = SkipWhiteSpace(token);

            using (Lexeme identifier = new Lexeme(LexemeType.Identifier))
            {
                identifier.AddToken(token);
                var next = this.scanner.ReadToken();

                while (next.TokenType() == TokenType.Character || next.TokenType() == TokenType.NumberDigit)
                {
                    identifier.AddToken(next);
                    next = this.scanner.ReadToken();
                }

                identifier.PrintLexeme("Identifier");
                this.scanner.PutTokenBack();

                return identifier;
            }

            throw new Exception();
        }

        private IToken SkipWhiteSpace(IToken token)
        {
            if (token.TokenType() == TokenType.WhiteSpace)
            {
                while (token.TokenType() == TokenType.WhiteSpace)
                {
                    token = scanner.ReadToken();
                }
            }

            return token;
        }

        private Lexeme GetNumber(IToken token)
        {
            Lexeme number = new Lexeme(LexemeType.Number);
            number.AddToken(token);

            var next = this.scanner.ReadToken();

            while(next.TokenType() == TokenType.Character || 
                  next.TokenType() == TokenType.NumberDigit)
            {
                number.AddToken(next);
                next = this.scanner.ReadToken();
            }

            this.scanner.PutTokenBack();

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

            if(symbolList.Contains(currentSymbol))
            {
                symbol.AddToken(token);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            if (currentSymbol.Equals('"'))
            {
                symbol.AddToken(token);
                var next = this.scanner.ReadToken();
                while (!next.GetTokenData().Equals('"'))
                {
                    symbol.AddToken(next);
                    next = this.scanner.ReadToken();
                }

                if (next.GetTokenData().Equals('"'))
                {
                    symbol.AddToken(next);
                    return symbol;
                }
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

            //symbol.PrintLexeme("Symbol");
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
                /*
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
                */
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(token),"No type of whitespace");
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
    }}


