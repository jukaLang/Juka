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
                if (token.TokenType() == TokenType.EofType)
                {
                    return;
                }

                if (token.TokenType() == TokenType.IdentifierType)
                {
                    VisitIdentifier(token);
                }

                Trace.Write(token.ToString());
            }
        }


        private IToken GetIdentifier(IToken token)
        {
            if (token.TokenType() == TokenType.WhiteSpaceType)
            {
                while (token.TokenType() == TokenType.WhiteSpaceType)
                {
                    token = scanner.ReadToken();
                }
            }

            StringBuilder identifier = new StringBuilder();
            identifier.Append(token.ToString());

            IToken next;

            while (true)
            {
                next = this.scanner.ReadToken();
                if (next.TokenType() == TokenType.SymbolType || next.TokenType() == TokenType.WhiteSpaceType || next.TokenType() == TokenType.PunctuationType)
                {
                    this.scanner.PutTokenBack();
                    break;
                }

                identifier.Append(next.ToString());
            }

            return new Identifier() { tokenIdentifierValue = identifier.ToString() };
        }

        private IToken GetPunctuation(IToken token)
        {
            while (token.TokenType() == TokenType.WhiteSpaceType)
            {
                token = scanner.ReadToken();
            }

            if (token.TokenType() == TokenType.PunctuationType)
            {
                return token;
            }

            throw new Exception("No Punctuation found;");
        }

        private IToken GetSymbol(IToken token)
        {
            while (token.TokenType() == TokenType.WhiteSpaceType)
            {
                token = scanner.ReadToken();
            }

            if (token.TokenType() == TokenType.SymbolType)
            {
                return token;
            }

            throw new Exception("No symbol found;");
        }

        private void VisitIdentifier(IToken token)
        {
            IToken tokenIdentifier = GetIdentifier(token);

            if (KeyWords.keyValuePairs.TryGetValue(tokenIdentifier.ToString(), out KeyWords.KeyWordsEnum keyWordsEnum))
            {
                if (keywordActions.ContainsKey(keyWordsEnum))
                {
                    keywordActions[keyWordsEnum](tokenIdentifier);
                }
            }
        }

        private void MainAction(IToken token)
        {
            if (token.TokenType() != TokenType.IdentifierType)
            {
                throw new Exception("no function name");
            }

            if (KeyWords.keyWordNames.Contains(token.ToString()))
            {
                if (token.ToString().Equals(KeyWords.FUNCTION))
                {

                    IToken functionName = GetIdentifier(scanner.ReadToken());
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

                    IToken isEqualSign = GetSymbol(scanner.ReadToken());
                    IToken isLeftBracket = GetPunctuation(scanner.ReadToken());
                }
            }
        }
    };
}


