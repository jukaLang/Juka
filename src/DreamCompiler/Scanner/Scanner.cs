using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Dynamic;
using Antlr4.Runtime;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using DreamCompiler.Tokens;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Char;

namespace DreamCompiler.Scanner
{
    public class Scanner
    {
        private int position;
        private byte[] fileData;
        
        public Scanner(string path)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                double fileLength = fileStream.Length;

                this.fileData = new byte[(int)fileLength];
                fileStream.Read(this.fileData, 0, (int)fileLength);
            }
        }


        internal IToken ReadToken()
        {
            TokenType tokenType = TokenType.NotValid;

            if (IsEOF())
            {
                tokenType = TokenType.Eof;
                return new Token(tokenType);
            }
            else
            {
                char t = (char) fileData[position++];

                if (IsLetter(t))
                {
                    tokenType = TokenType.Character;
                }
                else if (IsDigit(t) || IsNumber(t))
                {
                    tokenType = TokenType.NumberDigit;
                    //NumberDigit numberDigit = null;
                    //int value;
                    //if (int.TryParse(t.ToString(), out value))
                    //{
                    //    numberDigit = new NumberDigit() { tokenIntValue = value };
                    //}

                    //return numberDigit;
                }
                else if (IsPunctuation(t) || IsSymbol(t))
                {
                    tokenType = TokenType.Symbol;
                }
                else if (Char.IsWhiteSpace(t))
                {
                    tokenType = TokenType.WhiteSpace;
                }

                return new Token(tokenType, t);
            }
        }

        internal void PutTokenBack()
        {
            this.position--;
        }

        private bool IsEOF()
        {
            if (position >= fileData.Length)
            {
                return true;
            }

            return false;
        }

        internal bool IsWhiteSpace()
        {
            if (Char.IsWhiteSpace((char)fileData[position]))
            {
                return true;
            }

            return false;
        }

    }


    public enum TokenType
    {
        NotValid,
        Character,
        NumberDigit,
        WhiteSpace,
        Eof,
        Symbol,
    }

    internal enum LexemType
    {
        Identifier,
        Number,
    }

    public class Lexem
    {
        private List<IToken> tokenList = new List<IToken>();
        private LexemType lexemType;

        internal Lexem(LexemType ltype)
        {
            this.lexemType = ltype;
        }

        internal void AddToken(IToken token)
        {
            tokenList.Add(token);
        }

#if DEBUG
        internal void PrintLexem(string lexemType)
        {
            Trace.Write($"Lexem - Type:{lexemType} {{ ");
            foreach (var token in tokenList)
            {
                if (token is Token t)
                {
                    Trace.Write(t.data);
                }
            }

            Trace.Write(" }");

            Trace.WriteLine(string.Empty);

        }
#endif
    }


    public interface IToken
    {
        TokenType TokenType();

        char GetTokenData();
    }

    internal class Token : IToken
    {
        internal TokenType tokenType;
        internal char data;

        internal Token(TokenType t)
        {
            this.tokenType = t;
        }

        internal Token(TokenType t, char tokenData) : this(t)
        {
            this.data = tokenData;
        }

        public char GetTokenData()
        {
            return this.data;
        }

        public TokenType TokenType()
        {
            return this.tokenType;
        }
    }

 /*
    public abstract class TokenImpl : IToken
    {
        public string tokenStringValue { get; set; }

        public int tokenIntValue { get; set; }

        public string tokenIdentifierValue { get; set; }

        public string symbolValue { get; set; }

        public string whiteSpaceValue { get; set; }

        abstract public TokenType TokenType();
    }
   
    public class Identifier : TokenImpl
    {
        public override string ToString()
        {
            return this.tokenIdentifierValue;
        }

        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.IdentifierType;
        }
    }

    public class NumberDigit : TokenImpl
    {
        public override string ToString()
        {
            return this.tokenIntValue.ToString();
        }

        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.NumberDigitType;
        }
    }


    public class WhiteSpace : TokenImpl
    {
        public override string ToString()
        {
            return this.whiteSpaceValue.ToString();
        }

        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.WhiteSpaceType;
        }
    }

    public class Punctuation : TokenImpl
    {
        public string punctuationValue { get; set; }

        public override string ToString()
        {
            return this.punctuationValue;
        }
        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.PunctuationType;
        }
    }

   

    public class Eof : TokenImpl
    {
        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.EofType;
        }
    }

    public class Symbol : TokenImpl
    {
        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.SymbolType;
        }

        public override string ToString()
        {
            return this.symbolValue;
        }
    }
    */
}
