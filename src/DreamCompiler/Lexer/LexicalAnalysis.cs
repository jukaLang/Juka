using DreamCompiler.Scan;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DreamCompiler.Lexer
{
    public class LexicalAnalysis : ILexicalAnalysis
    {
        private IScanner scanner;
        public Dictionary<KeyWords.KeyWordsEnum, Action<IToken>> keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>();
        private readonly List<char> symbolList = new List<char>() { '=', '/', '-', ',', ';', '(', ')', '{', '}' };

        public LexicalAnalysis()
        {
            keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>()
            {
                {KeyWords.KeyWordsEnum.Main, MainAction},
                {KeyWords.KeyWordsEnum.Function, MainAction },
            };
        }

        public LexemeListManager Analyze(IScanner scanner)
        {
            this.scanner = scanner;

            List<Lexeme> lexemeList = new List<Lexeme>();
            var errorList = new List<LexicalAnalysisException>();

            while (true)
            {
                IToken token = scanner.ReadToken();
                if (token == null || token.TokenType() == TokenType.Eof || token.TokenType() == TokenType.NotValid)
                {
                    break;
                }

                try
                {
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
                catch (LexicalAnalysisException lae)
                {
                    errorList.Add(lae);
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

                while (next != null && (next.TokenType() == TokenType.Character || next.TokenType() == TokenType.NumberDigit))
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
            Lexeme symbol = new Lexeme(LexemeType.Symbol);

            var currentSymbol = token.GetTokenData();

            if(currentSymbol.Equals(';'))
            {
                symbol.AddToken(token);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            if (currentSymbol.Equals('(') || currentSymbol.Equals(')'))
            {
                symbol.AddToken(token);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            if (currentSymbol.Equals('"'))
            {
                return GetString(token, symbol, '"');
            }

            if (currentSymbol.Equals('\''))
            {
                return GetCharacterLiteral(token, symbol, '\'');
            }

            if (currentSymbol.Equals('+'))
            {
                symbol.AddToken(token);
                var next = this.scanner.ReadToken();
                
                if (next.GetTokenData().Equals('+'))
                {
                    symbol.AddToken(next);
                    symbol.PrintLexeme("Symbol");
                    return symbol;
                }

                symbol.PrintLexeme("Symbol");
                this.scanner.PutTokenBack();
                return symbol;
            }

            if (currentSymbol == '{')
            {
                TryGetMultiCharSymbol(symbol, token, '{');
            }

            if (currentSymbol == '}')
            {
                TryGetMultiCharSymbol(symbol, token, '}');
            }

            if (currentSymbol == '=')
            {
                TryGetMultiCharSymbol(symbol, token, '=');
            }

            if (currentSymbol == '<' || currentSymbol == '>')
            {
                TryGetMultiCharSymbol(symbol, token, '=');
            }


            return symbol;
        }

        private Lexeme GetString(IToken token, Lexeme symbol, char str)
        {
            symbol.AddToken(token);
            var next = this.scanner.ReadToken();
            while (!next.GetTokenData().Equals(str))
            {
                symbol.AddToken(next);
                next = this.scanner.ReadToken();
            }

            if (next.GetTokenData().Equals(str))
            {
                symbol.AddToken(next);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            throw new LexicalAnalysisException();
        }

        private Lexeme GetCharacterLiteral(IToken token, Lexeme symbol, char str)
        {
            int count = 0;
            symbol.AddToken(token);
            var next = this.scanner.ReadToken();
            while (!next.GetTokenData().Equals(str))
            {
                if (count > 1)
                {
                    throw new LexicalAnalysisException("Too many characters in character literal");
                }

                symbol.AddToken(next);
                next = this.scanner.ReadToken();
                count++;
            }

            if (next.GetTokenData().Equals(str))
            {
                symbol.AddToken(next);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            throw new LexicalAnalysisException();

        }

        private void TryGetMultiCharSymbol(Lexeme symbol, IToken token, char nextSymbol)
        {
            symbol.AddToken(token);
            while (true)
            {
                var next = this.scanner.ReadToken();
                if (next.GetTokenData() == nextSymbol)
                {
                    symbol.AddToken(next);
                    symbol.PrintLexeme("Symbol");
                    break;
                }
                else
                {
                    this.scanner.PutTokenBack();
                    symbol.PrintLexeme("Symbol");
                    break;
                }
            }
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


