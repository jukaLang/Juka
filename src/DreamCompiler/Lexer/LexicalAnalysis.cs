using DreamCompiler.Scanner;
using DreamCompiler.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.Lexer
{
    public class LexicalAnalysis
    {
        private Scanner scanner;
        public Dictionary<KeyWords.KeyWordsEnum, Action<IToken>> keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>();
        public LexicalAnalysis(Scanner scanner)
        {
            this.scanner = scanner;

            keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<IToken>>()
            {
                {KeyWords.KeyWordsEnum.Main, MainAction},
                {KeyWords.KeyWordsEnum.Function, MainAction },
            };
        }

        public void Analyze()
        {
            while (true)
            {
                IToken token = scanner.ReadToken();
                if (token.TokenType() == TokenType.Eof)
                {
                    return;
                }

                switch (token.TokenType())
                {
                    case TokenType.Character:
                    {
                        GetIdentifier(token);
                        break;
                    }

                    case TokenType.NumberDigit:
                    {
                        GetNumber(token);
                        break;
                    }

                    case TokenType.Symbol:
                    {
                        GetSymbol(token);
                        break;
                    }
                }
            }
        }


        private Lexem GetIdentifier(IToken token)
        {
            if (token.TokenType() == TokenType.WhiteSpace)
            {
                while (token.TokenType() == TokenType.WhiteSpace)
                {
                    token = scanner.ReadToken();
                }
            }

            Lexem identifier = new Lexem(LexemType.Identifier);
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

            identifier.PrintLexem("Identifier");

            return identifier;
        }


        private Lexem GetNumber(IToken token)
        {
            Lexem number = new Lexem(LexemType.Number);
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

            number.PrintLexem("Number");
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

        private Lexem GetSymbol(IToken token)
        {
            Lexem symbol = new Lexem(LexemType.Number);


            var currentSymbol = token.GetTokenData();
            if (currentSymbol == '(' ||
                currentSymbol == ')' ||
                currentSymbol == '"' ||
                currentSymbol == '{' ||
                currentSymbol == '}'
                )
            {
                symbol.AddToken(token);
                symbol.PrintLexem("Symbol");
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

            symbol.PrintLexem("Symbol");
            return symbol;
        }

        private void VisitIdentifier(IToken token)
        {
            Lexem tokenIdentifier = GetIdentifier(token);

            /*
            if (KeyWords.keyValuePairs.TryGetValue(tokenIdentifier.ToString(), out KeyWords.KeyWordsEnum keyWordsEnum))
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

                    Lexem functionName = GetIdentifier(scanner.ReadToken());
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

                    Lexem isEqualSign = GetSymbol(scanner.ReadToken());
                    IToken isLeftBracket = GetPunctuation(scanner.ReadToken());
                }
            }
        }
    };
}


