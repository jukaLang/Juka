using JukaCompiler.Lexer;
using static System.Char;

namespace JukaCompiler.Scan
{
    internal class Scanner
    {
        private int position = 0;
        private int line = 0;
        private byte[] fileData;
        private List<Lexeme> lexemes = new List<Lexeme>();

        private static readonly Dictionary<string, Int64> keywordsDictionary = new Dictionary<string, Int64>
        {
            { "and",    LexemeType.AND },
            { "class",  LexemeType.CLASS },
            { "else",   LexemeType.ELSE },
            { "func",   LexemeType.FUNC },
            { "for",    LexemeType.FOR },
            { "if",     LexemeType.IF },
            { "null",   LexemeType.NULL },
            { "or",     LexemeType.OR },
            { "return", LexemeType.RETURN },
            { "super",  LexemeType.SUPER },
            { "this",   LexemeType.THIS },
            { "true",   LexemeType.TRUE },
            { "var",    LexemeType.VAR },
            { "while",  LexemeType.WHILE },
            { "int",    LexemeType.INT }
        };

        private static readonly Dictionary<string, Int64> internalFunctionsList = new Dictionary<string, Int64>
        {
            {"printLine", LexemeType.PRINTLINE},
        };

        internal Scanner(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("The path is null");
            }

            fileData = File.ReadAllBytes(path);
        }

        internal List<Lexeme> Scan()
        {
            while(!IsEof())
            {
                ReadToken();
            }

            return lexemes;
        }

        internal bool IsEof()
        {
            if (position == fileData.Length)
            {
                return true;
            }

            return false;
        }

        internal void ReadToken()
        {
            if (IsEof())
            {
                return;
            }

            char t = (char) fileData[position];

            if (IsLetter(t))
            {
                Identifier(LexemeType.IDENTIFIER);
                return;
            }

            if (IsDigit(t) || IsNumber(t))
            {
                Number(LexemeType.NUMBER);
                return;
            }

            if (IsPunctuation(t) || IsSymbol(t))
            {
                switch(t)
                {
                    case '>': AddSymbol( t, LexemeType.GREATER); break;
                    case '<': AddSymbol( t, LexemeType.LESS); break;
                    case '(': AddSymbol( t, LexemeType.LEFT_PAREN); break;
                    case ')': AddSymbol( t, LexemeType.RIGHT_PAREN); break;
                    case '{': AddSymbol( t, LexemeType.LEFT_BRACE); break;
                    case '}': AddSymbol( t, LexemeType.RIGHT_BRACE); break;
                    case ',': AddSymbol( t, LexemeType.COMMA); break;
                    case '.': AddSymbol( t, LexemeType.DOT); break;
                    case '-': AddSymbol( t, LexemeType.EQUAL); break;
                    case '+': AddSymbol( t, LexemeType.PLUS); break;
                    case ';': AddSymbol( t, LexemeType.SEMICOLON); break;
                    case '*': AddSymbol( t, LexemeType.STAR); break;
                    case '=': AddSymbol( t, LexemeType.EQUAL); break;

                        /*
                        case '!':
                            { 
                                if (Match('='))
                                { 
                                    kind = LexemeType.BANG_EQUAL;
                                    break;
                                }

                                kind = LexemeType.BANG;
                                break;
                            }
                    position++;
                    */
                        case '"' : String(); break;
                }

                position++;
            }

            if (Char.IsWhiteSpace(t))
            {
                position++;
            }
        }

        internal void AddSymbol(char symbol, Int64 type)
        {
            var lex = new Lexeme(type);
            lex.AddToken(symbol);
            this.lexemes.Add(lex);
        }

        internal bool TryGetKeyWord(Lexeme lex)
        {
            bool isKeyword = false;

            if (keywordsDictionary.TryGetValue(lex.ToString(), out var lexemeType))
            {
                isKeyword = lex.IsKeyWord = true;
                lex.LexemeType = lexemeType;
            }

            if (internalFunctionsList.ContainsKey(lex.ToString()))
            {
                lex.LexemeType = internalFunctionsList[lex.ToString()];
                lex.LexemeType |= LexemeType.INTERNALFUNCTION;
            }

            return isKeyword;
        }

        internal bool IsWhiteSpace()
        {
            char c = (char)fileData[position];
            if (Char.IsWhiteSpace((char) c) || c == '\r' || c == '\n')
            {
                return true;
            }

            return false;
        }

        internal char CurrentChar()
        {
            return (char)fileData[position];
        }

        internal bool Match(char expected)
        {
            if (IsEof())
            {
                return false;
            }

            if ((char)fileData[position] != expected)
            {
                return false;
            }

            position++;
            return true;
        }

        internal void Identifier(Int64 kind)
        {
            Lexeme identifier = new Lexeme(LexemeType.IDENTIFIER);

            while (IsLetterOrDigit(CurrentChar()))
            {
                identifier.AddToken(CurrentChar());
                Advance();
            }

            TryGetKeyWord(identifier);

            this.lexemes.Add(identifier);
        }

        internal void Number(Int64 kind)
        {
            Lexeme number = new Lexeme(kind);

            while((IsNumber(CurrentChar())))
            {
                number.AddToken(CurrentChar());
                Advance();
            }

            this.lexemes.Add(number);
        }

        private void String()
        {
            Advance();
            int start = position;
            while( Peek() != '"' && !IsEof())
            {
                if (Peek() == '\n')
                {
                    this.line++;
                }
                
                Advance();
            }

            if(IsEof())
            {
                // Log exception;
                return;
            }

            var mem = Memcopy(fileData, start, position);

            Lexeme s = new Lexeme(LexemeType.STRING);
            foreach(byte b in mem)
            {
                s.AddToken((char)b);
            }

            this.lexemes.Add(s);

            Advance();
        }

        private byte[] Memcopy(byte[] from, int start, int size)
        {
            byte [] to = new byte[position - start];
            for(int toIndex = 0, i = start; i < position; i++, toIndex++)
            {
                to[toIndex] = from[i];
            }

            return to;
        }

        private void Advance()
        {
            if (!IsEof())
            {
                position++;
            }
        }

        internal char Peek()
        {
            if (IsEof())
            {
                return '\0';
            }

            return (char)fileData[position];
        }

        internal void Reverse()
        {
            if (position - 1 > 0)
            {
                this.position--;
            }
        }
    }
}

