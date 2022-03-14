using JukaCompiler.Lexer;
using static System.Char;
using System.Text;
using JukaCompiler.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Scan
{
    internal class Scanner
    {
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private int column = 0;
        private byte[] fileData;
        private List<Lexeme> lexemes = new List<Lexeme>();
        private ICompilerError compilerError = null;

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
            { "false",  LexemeType.FALSE },
            { "true",   LexemeType.TRUE },
            { "var",    LexemeType.VAR },
            { "while",  LexemeType.WHILE },
            { "int",    LexemeType.INT },
            { "char",   LexemeType.CHAR },
            { "string", LexemeType.STRING },
            { "break",  LexemeType.BREAK },
        };

        private static readonly Dictionary<string, Int64> internalFunctionsList = new Dictionary<string, Int64>
        {
            {"printLine", LexemeType.PRINTLINE},
            {"print", LexemeType.PRINT}
        };

        internal Scanner(string data, IServiceProvider serviceProvider, bool isFile = true)
        {
            this.compilerError = serviceProvider.GetRequiredService<ICompilerError>();
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
            this.column++;
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
                    case '(': AddSymbol( t, LexemeType.LEFT_PAREN); break;
                    case ')': AddSymbol( t, LexemeType.RIGHT_PAREN); break;
                    case '{': AddSymbol( t, LexemeType.LEFT_BRACE); break;
                    case '}': AddSymbol( t, LexemeType.RIGHT_BRACE); break;
                    case ',': AddSymbol( t, LexemeType.COMMA); break;
                    case '.': AddSymbol( t, LexemeType.DOT); break;
                    case '-': AddSymbol( t, LexemeType.MINUS); break;
                    case '+': AddSymbol( t, LexemeType.PLUS); break;
                    case ';': AddSymbol( t, LexemeType.SEMICOLON); break;
                    case '*': AddSymbol( t, LexemeType.STAR); break;

                    case '=':
                        {
                            if( Peek() == '=') 
                            {
                                AddSymbol( t ,LexemeType.EQUAL_EQUAL);
                                break;
                            }

                            AddSymbol(t ,LexemeType.EQUAL);
                            break;
                        }

                    case '<':
                        {
                            if (Peek() == '=')
                            {
                                AddSymbol(t, LexemeType.LESS_EQUAL);
                                break;
                            }

                            AddSymbol(t, LexemeType.LESS); 
                            break;
                        }

                    case '>':
                        {
                            if (Peek() == '=')
                            {
                                AddSymbol(t, LexemeType.GREATER_EQUAL);
                                break;
                            }

                            AddSymbol(t, LexemeType.GREATER);
                            break;
                        }


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

            IsWhiteSpace();
        }

        internal void AddSymbol(char symbol, Int64 type)
        {
            var lex = new Lexeme(type, this.line, this.column);
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
            if (IsEof())
            {
                return false;
            }

            char c = (char)fileData[current];
            if (Char.IsWhiteSpace((char) c) || c == '\r' || c == '\n')
            {
                if (c == '\n')
                {
                    this.line++;
                    this.column = 0;
                }

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
            Lexeme identifier = new Lexeme(LexemeType.IDENTIFIER, this.line, this.column);
            
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
            Lexeme number = new Lexeme(LexemeType.NUMBER, this.line, this.column);

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
            Lexeme s = new Lexeme(LexemeType.STRING, this.line, this.column);
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

