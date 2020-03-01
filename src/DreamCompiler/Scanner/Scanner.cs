using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using DreamCompiler.Lexer;
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

                this.fileData = new byte[(int) fileLength];
                fileStream.Read(this.fileData, 0, (int) fileLength);
            }
        }

        public Scanner(MemoryStream memoryStream)
        {
            int memoryStreamLength = (int) memoryStream.Length;

            fileData = new byte[memoryStreamLength];

            int dataRead = memoryStream.Read(fileData, 0, memoryStreamLength);

            if (dataRead != memoryStreamLength)
            {
                throw new Exception("bad memory read");
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
            if (Char.IsWhiteSpace((char) fileData[position]))
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

    public enum LexemeType
    {
        Identifier,
        Number,
        WhiteSpace,
    }

    public class Lexeme : IDisposable
    {
        private List<IToken> tokenList = new List<IToken>();
        private LexemeType lexemeType;
        private bool isKeyWord;
        private string tokenAsString = String.Empty;
        // ReSharper disable once InconsistentNaming
        private KeyWords.KeyWordsEnum keyWordType;

        internal Lexeme(LexemeType ltype)
        {
            this.lexemeType = ltype;
        }

        internal void AddToken(IToken token)
        {
            tokenList.Add(token);
        }

        public bool IsKeyWord()
        {
            return isKeyWord;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(tokenAsString))
            {
                var s = new StringBuilder();
                foreach (var t in tokenList)
                {
                    s.Append(t.GetTokenData());
                }

                return s.ToString();
            }

            return tokenAsString;
        }

        public LexemeType LexemeType => lexemeType;

        internal KeyWords.KeyWordsEnum KeyWordType => keyWordType;

        void IDisposable.Dispose()
        {
            this.tokenAsString = ToString();

            if (lexemeType == LexemeType.Identifier)
            {
                if (KeyWords.keyValuePairs.TryGetValue(this.tokenAsString, out KeyWords.KeyWordsEnum keyWordValue))
                {
                    isKeyWord = true;
                    this.keyWordType = keyWordValue;
                }
            }
        }

#if DEBUG
        internal void PrintLexeme(string lexemeType, Action action = null)
        {
            Trace.Write($"Lexeme - Type:{lexemeType} {{ '");

            if (action == null)
            {
                foreach (var token in tokenList)
                {
                    if (token is Token t)
                    {
                        Trace.Write(t.data);
                    }
                }
            }
            else
            {
                action();
            }

            Trace.Write("' }");

            Trace.WriteLine(string.Empty);

        }
#endif
    }


    public class LexemeListManager : IEnumerable
    {
        private List<Lexeme> lexemList;

        public LexemeListManager(List<Lexeme> list)
        {
            this.lexemList = list;
        }

        /*
        public Lexeme NextNotWhiteSpace(out Lexeme lexeme)
        {
            while (lexemList[currentLexem].LexemeType == LexemeType.WhiteSpace)
            {
                currentLexem++;
            }

            lexeme = lexemList[currentLexem];
            return lexeme;
        }
        */

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        public LexemeEnumerator GetEnumerator()
        {
            return new LexemeEnumerator(this.lexemList);
        }

    }


    public class LexemeEnumerator : IEnumerator
    {
        private List<Lexeme> lexemeList;
        private int position = -1;

        public LexemeEnumerator(List<Lexeme> list)
        {
            lexemeList = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < lexemeList.Count);
        }

        public Lexeme MoveNextSkipWhite()
        {
            if (position < 0)
            {
                position++;
            }

            while (lexemeList[position].LexemeType == LexemeType.WhiteSpace)
            {
                position++;
            }

            if (position < lexemeList.Count)
            {
                return lexemeList[position++];
            }

            throw new InvalidOperationException();
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current => Current;

        public Lexeme Current
        {
            get
            {
                try
                {
                    return lexemeList[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
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
