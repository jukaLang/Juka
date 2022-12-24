using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using static System.Char;

namespace JukaCompiler.Scan
{
    internal class Scanner
    {
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private int column = 0;
        private byte[] fileData;
        private readonly List<Lexeme?> lexemes = new();
        private ICompilerError compilerError;

        private static readonly Dictionary<string, LexemeType.Types> keywordsDictionary = new()
        {
            { "and",    LexemeType.Types.AND },
            { "class",  LexemeType.Types.CLASS },
            { "else",   LexemeType.Types.ELSE },
            { "func",   LexemeType.Types.FUNC },
            { "for",    LexemeType.Types.FOR },
            { "if",     LexemeType.Types.IF },
            { "null",   LexemeType.Types.NULL },
            { "or",     LexemeType.Types.OR },
            { "return", LexemeType.Types.RETURN },
            { "super",  LexemeType.Types.SUPER },
            { "this",   LexemeType.Types.THIS },
            { "false",  LexemeType.Types.FALSE },
            { "true",   LexemeType.Types.TRUE },
            { "var",    LexemeType.Types.VAR },
            { "while",  LexemeType.Types.WHILE },
            { "int",    LexemeType.Types.INT },
            { "char",   LexemeType.Types.CHAR },
            { "string", LexemeType.Types.STRING },
            { "break",  LexemeType.Types.BREAK },
            { "array",  LexemeType.Types.ARRAY},
            { "new"  ,  LexemeType.Types.NEW},
            { "delete", LexemeType.Types.DELETE}
        };

        private static readonly Dictionary<string, LexemeType.Types> internalFunctionsList = new()
        {
            {"print", LexemeType.Types.PRINT},
            {"printLine", LexemeType.Types.PRINTLINE}
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

        internal List<Lexeme?> Scan()
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
                    case '(':
                    {
                        AddSymbol( t, LexemeType.Types.LEFT_PAREN);
                       
                        if (IsDigit(Peek()) || IsNumber(Peek()))
                        {
                            // HACK
                            start++;
                            // HACK
                            Number();
                            /*if (Peek() == ')')
                            {
                                AddSymbol(t, LexemeType.Types.RIGHT_PAREN);
                            }*/
                        }

                        

                        break;
                    }
                    case ')': AddSymbol( t, LexemeType.Types.RIGHT_PAREN); break;
                    case '{': AddSymbol(t, LexemeType.Types.LEFT_BRACE); break;
                    case '}': AddSymbol( t, LexemeType.Types.RIGHT_BRACE); break;
                    case ',': AddSymbol( t, LexemeType.Types.COMMA); break;
                    case '.': AddSymbol( t, LexemeType.Types.DOT); break;
                    case '+': AddSymbol( t, LexemeType.Types.PLUS); break;
                    case ';': AddSymbol( t, LexemeType.Types.SEMICOLON); break;
                    case '*': AddSymbol( t, LexemeType.Types.STAR); break;
                    case '[':
                    {
                        AddSymbol(t, LexemeType.Types.LEFT_BRACE);
                        if (IsDigit(Peek()) || IsNumber(Peek()))
                        {
                            // HACK
                            start++;
                            // HACK
                            Number();
                        }

                        if (Peek() == ']')
                        {
                            AddSymbol(Peek(), LexemeType.Types.RIGHT_BRACE);
                        }

                        break;
                    }
                    case '-':
                    {
                        if (IsDigit(Peek()) || IsNumber(Peek()))
                        {
                            //Debug.WriteLine("TEST");
                            //Debug.WriteLine(Peek());
                            Number();
                            //Debug.WriteLine(Peek());
                            //Debug.WriteLine("TRACED+1");
                        } else
                        {
                            AddSymbol(t, LexemeType.Types.MINUS);
                            //Debug.WriteLine("TEST2");
                            //Debug.WriteLine(Peek());
                            //Debug.WriteLine("TRACED+SYMBOL");
                        }

                        break;
                    }
                    case '/':
                        if (Peek() == '/')
                        {
                            while (Peek() != '\n' && !IsEof())
                            {
                                Advance();
                            }
                        }
                        else if (Peek() == '*')
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
                        else
                        {
                            AddSymbol(t, LexemeType.Types.SLASH);
                        }
                        break;


                    case '=':
                        {
                            if( Peek() == '=') 
                            {
                                AddSymbol( t ,LexemeType.Types.EQUAL_EQUAL);
                                break;
                            }

                            AddSymbol(t ,LexemeType.Types.EQUAL);
                            break;
                        }

                    case '<':
                        {
                            if (Peek() == '=')
                            {
                                AddSymbol(t, LexemeType.Types.LESS_EQUAL);
                                break;
                            }

                            AddSymbol(t, LexemeType.Types.LESS); 
                            break;
                        }

                    case '>':
                        {
                            if (Peek() == '=')
                            {
                                AddSymbol(t, LexemeType.Types.GREATER_EQUAL);
                                break;
                            }

                            AddSymbol(t, LexemeType.Types.GREATER);
                            break;
                        }


                    /*
                    case '!':
                        { 
                            if (Match('='))
                            { 
                                kind = LexemeType.Types.BANG_EQUAL;
                                break;
                            }

                            kind = LexemeType.Types.BANG;
                            break;
                        }
                position++;
                */
                    case '"' : String(); break;
                    default:
                        //Lox.error(line, "Unexpected character.");
                        break;
                }
            }

            IsWhiteSpace();
        }

        internal void AddSymbol(char symbol, LexemeType.Types type)
        {
            var lex = new Lexeme(type, this.line, this.column);
            lex.AddToken(symbol);
            this.lexemes.Add(lex);
        }

        internal bool TryGetKeyWord(Lexeme? lex)
        {
            bool isKeyword = false;

            if (keywordsDictionary.TryGetValue(lex?.ToString()!, out var lexemeType))
            {
                if (lex != null)
                {
                    isKeyword = lex.IsKeyWord = true;
                    lex.LexemeType = lexemeType;
                }
            }

            if (internalFunctionsList.ContainsKey(lex?.ToString()!))
            {
                if (lex != null)
                {
                    lex.LexemeType = internalFunctionsList[lex.ToString()];
                    lex.LexemeType |= LexemeType.Types.INTERNALFUNCTION;
                }
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
        internal void Identifier()
        {
            while (IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                Advance();
            }

            var svalue = Encoding.Default.GetString(Memcopy(fileData, start, current));
            Lexeme? identifier = new(LexemeType.Types.IDENTIFIER, this.line, this.column);
            
            identifier.AddToken(svalue);

            TryGetKeyWord(identifier);
            this.lexemes.Add(identifier);
        }
        internal void Number()
        {
            char temp = Peek();

            while (IsNumber(temp) || temp.Equals('.'))
            {
                Advance();
                temp = Peek();
            }

            var svalue = System.Text.Encoding.Default.GetString(Memcopy(fileData, start, current));
            Lexeme? number = new(LexemeType.Types.NUMBER, this.line, this.column);

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
            Lexeme? s = new(LexemeType.Types.STRING, this.line, this.column);
            s.AddToken(svalue.ToString());
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
        internal char Peek()
        {
            if (!IsEof())
            {
                return (char)fileData[current]; 
            }
            return '\0';
        }

    }
}

