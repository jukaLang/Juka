using DreamCompiler.Lexer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using static System.Char;

namespace DreamCompiler.Scanner
{
    public class Scanner : IScanner
    {
        private int position = 0;
        private byte[] fileData;
        private long bufferLength = 0;
        private ICommandLineProvider provider;
        private string path;
        private int bufferOffset = 0;
        private int bufferCount = 100;
        private int totalBytesRead = 0;
        private MemoryMappedViewStream viewStream;

        public Scanner(ICommandLineProvider provider)
        {
            this.provider = provider;
            this.path = this.provider.Check(0);
            BufferInitialLoad();
            TryReadBufferBytes();
        }

        public void BufferInitialLoad()
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
            {
                this.viewStream = mmf.CreateViewStream();
                this.bufferLength = viewStream.Length;
                fileData = new byte[viewStream.Length];
            }
        }

        public bool TryReadBufferBytes()
        { 
            if (this.viewStream.CanRead)
            {
                this.totalBytesRead += viewStream.Read(fileData, bufferOffset , bufferCount);
                return true;
            }

            return false;
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

        public IToken ReadToken()
        {
            TokenType tokenType = TokenType.NotValid;

            if (position == fileData.Length)
            {
                return new Token(TokenType.Eof);
            }

            if (position >= this.totalBytesRead)
            {
                bufferOffset = position;
                TryReadBufferBytes();
            }

            char t = (char) fileData[position];

            if (IsLetter(t))
            {
                tokenType = TokenType.Character;
                position++;
                return new Token(tokenType, t);
            }

            if (IsDigit(t) || IsNumber(t))
            {
                tokenType = TokenType.NumberDigit;
                position++;
                return new Token(tokenType, t);
            }

            if (IsPunctuation(t) || IsSymbol(t))
            {
                tokenType = TokenType.Symbol;
                position++;
                return new Token(tokenType, t);
            }

            if (Char.IsWhiteSpace(t))
            {
                tokenType = TokenType.WhiteSpace;
                position++;
                return new Token(tokenType, t);
            }

            position++;
            return new Token(tokenType, t);
        }

        public void PutTokenBack()
        {
            this.position--;
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
        Symbol,
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
            //position++;
            //return (position < lexemeList.Count);
            throw new NotImplementedException();
        }

        public bool MoveNextEx()
        {
            position++;
            if (lexemeList[position].LexemeType == LexemeType.WhiteSpace)
            {
                return MoveNextEx();
            }

            if (position < lexemeList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }

            throw new InvalidOperationException();
        }

        public bool MoveBackEx()
        {
            position--;
            if (lexemeList[position].LexemeType == LexemeType.WhiteSpace)
            {
                return MoveNextEx();
            }

            if (position < 0)
            {
                return false;
            }

            if (position >= 0)
            {
                return true;
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

    public class Token : IToken
    {
        internal TokenType tokenType;
        internal char data;

        public Token(TokenType t)
        {
            this.tokenType = t;
        }

        public Token(TokenType t, char tokenData) : this(t)
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

}
