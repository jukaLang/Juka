using JukaCompiler.Lexer;
using static System.Char;
using System.Text;

namespace JukaCompiler.Scan
{
    // Step 4: Scan the string
    internal class Scanner
    {
        private int start = 0;
        private int current = 0;
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
            { "int",    LexemeType.INT },
            { "char",   LexemeType.CHAR },
        };

        private static readonly Dictionary<string, Int64> internalFunctionsList = new Dictionary<string, Int64>
        {
            {"printLine", LexemeType.PRINTLINE},
        };

        internal Scanner(string data, bool isFile = true)
        {
            if (isFile)
            {
                if (string.IsNullOrEmpty(data))
                {
                    throw new ArgumentNullException("The path is null");
                }

                if (!File.Exists(data))
                {
                    throw new FileLoadException("Unable to find file " + data);
                }

                fileData = File.ReadAllBytes(data);
                return;
            }

            fileData = Encoding.ASCII.GetBytes(data);
        }

        internal List<Lexeme> Scan()
        {
            while(!IsEof())
            {
                start = current;
                ReadToken();
            }

            return lexemes;
        }

        internal bool IsEof()
        {
            if (current == fileData.Length)
            {
                return true;
            }

            return false;
        }


        internal void ReadToken()
        {
            char t = Advance();

            // Comments
            if (t != '\\' && Peek() == '/')
            {
                Comment();
                return;
            }

            if (IsLetter(t) || t == '_')
            {
                Identifier();
                return;
            }

            if (IsDigit(t) || IsNumber(t))
            {
                Number();
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
            }


            // Comments
            if (Prev() != '\\' && t == '/')
            {
                Comment();
                return;
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
            char c = (char)fileData[current];
            if (Char.IsWhiteSpace((char) c) || c == '\r' || c == '\n')
            {
                return true;
            }

            return false;
        }

        internal char CurrentChar()
        {
            return (char)fileData[current];
        }

        internal bool Match(char expected)
        {
            if (IsEof())
            {
                return false;
            }

            if ((char)fileData[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        internal void Identifier()
        {
            while (IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                Advance();
            }

            var svalue = Encoding.Default.GetString(Memcopy(fileData, start, current));
            Lexeme identifier = new Lexeme(LexemeType.IDENTIFIER);
            
            identifier.AddToken(svalue);

            TryGetKeyWord(identifier);
            this.lexemes.Add(identifier);
        }

        internal void Comment()
        {
            if (Peek() == '/')
            {
                while(Peek() != '\n' && !IsEof())
                {
                    Advance();
                }
            } else if(Peek() == '*')
            {
                while (true)
                {
                    if (Advance() == '*' && Peek() == '/')
                    {
                        Advance();
                        break;
                    }
                    if (IsEof())
                    {
                        throw new Exception("Comment is not closed");
                    }
                }
            }
        }

        internal void Number()
        {
            while(IsNumber(Peek()))
            {
                Advance();
            }

            var svalue = System.Text.Encoding.Default.GetString(Memcopy(fileData, start, current));
            Lexeme number = new Lexeme(LexemeType.NUMBER);

            number.AddToken(svalue);
            this.lexemes.Add(number);
        }

        private void String()
        {
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

            var svalue = System.Text.Encoding.Default.GetString(Memcopy(fileData, start + 1, current - 1));
            Lexeme s = new Lexeme(LexemeType.STRING);
            s.AddToken(svalue);
            this.lexemes.Add(s);

            Advance();
        }

        private byte[] Memcopy(byte[] from, int start, int size)
        {
            byte [] to = new byte[current - start];
            for(int toIndex = 0, i = start; i < current; i++, toIndex++)
            {
                to[toIndex] = from[i];
            }

            return to;
        }

        private char Advance()
        {
            if (!IsEof())
            {
                return (char)fileData[current++];
            }

            return '\0';
        }

        internal char Prev()
        {
            if ((current - 1) >= 0)
            {
                return (char)fileData[current-1];
            }

            return '\n';
        }


        internal char Peek()
        {
            if (!IsEof())
            {
                return (char)fileData[current]; 
            }
            return '\0';
        }

        internal void Reverse()
        {
            if (current - 1 > 0)
            {
                this.current--;
            }
        }
    }
}

