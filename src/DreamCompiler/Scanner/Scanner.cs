using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using DreamCompiler.Tokens;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DreamCompiler.Scanner
{
    public class Scanner
    {
        private int position;
        byte[] fileData;
        
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
            if (IsEOF())
            {
                return new Eof();
            }

            char t = (char)fileData[position++];

            if (Char.IsLetter(t))
            {
                return new Identifier() { tokenIdentifierValue = t.ToString() };
            }

            if (Char.IsDigit(t) || Char.IsNumber(t))
            {
                NumberDigit numberDigit = null;
                int value;
                if (int.TryParse(t.ToString(), out value))
                {
                    numberDigit = new NumberDigit() { tokenIntValue = value };
                }

                return numberDigit;
            }

            if (Char.IsPunctuation(t))
            {
                return new Punctuation() { punctuationValue = t.ToString() };
            }

            if (Char.IsSymbol(t))
            {
                return new Symbol() { symbolValue  = t.ToString() };
            }

            if (char.IsWhiteSpace(t))
            {
                return new WhiteSpace() { whiteSpaceValue = t.ToString() };
            }

            throw new InvalidOperationException($"no operator for token type char {t}");
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
        StringType,
        IdentifierType,
        NumberDigitType,
        WhiteSpaceType,
        PunctuationType,
        EofType,
        SymbolType,
    }

    public interface IToken
    {
        TokenType TokenType();
    }


    public abstract class TokenImpl : IToken
    {
        public string tokenStringValue { get; set; }

        public int tokenIntValue { get; set; }

        public string tokenIdentifierValue { get; set; }

        public string symbolValue { get; set; }

        public string whiteSpaceValue { get; set; }

        abstract public TokenType TokenType();
    }

    public class StringToken : TokenImpl
    {
        public override string ToString()
        {
            return this.tokenStringValue;
        }

        public override TokenType TokenType()
        {
            return DreamCompiler.Scanner.TokenType.StringType;
        }
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
}
