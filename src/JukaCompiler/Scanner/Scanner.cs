using System.Diagnostics;
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
        private readonly List<Lexeme?> lexemes = new List<Lexeme?>();
        private ICompilerError compilerError;

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
            { "array", LexemeType.ARRAY},
        };

        private static readonly Dictionary<string, Int64> internalFunctionsList = new Dictionary<string, Int64>
        {
            {"print", LexemeType.PRINT},
            {"printLine", LexemeType.PRINTLINE}
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
                        AddSymbol( t, LexemeType.LEFT_PAREN);
                       
                        if (IsDigit(Peek()) || IsNumber(Peek()))
                        {
                            // HACK
                            start++;
                            // HACK
                            Number();
                            if (Peek() == ')')
                            {
                                AddSymbol(t, LexemeType.RIGHT_PAREN);
                            }
                        }

                        

                        break;
                    }
                    case ')': AddSymbol( t, LexemeType.RIGHT_PAREN); break;
                    case '{': AddSymbol(t, LexemeType.LEFT_BRACE); break;
                    case '}': AddSymbol( t, LexemeType.RIGHT_BRACE); break;
                    case ',': AddSymbol( t, LexemeType.COMMA); break;
                    case '.': AddSymbol( t, LexemeType.DOT); break;
                    case '+': AddSymbol( t, LexemeType.PLUS); break;
                    case ';': AddSymbol( t, LexemeType.SEMICOLON); break;
                    case '*': AddSymbol( t, LexemeType.STAR); break;
                    case '[':
                    {
                        AddSymbol(t, LexemeType.LEFT_BRACE);
                        if (IsDigit(Peek()) || IsNumber(Peek()))
                        {
                            // HACK
                            start++;
                            // HACK
                            Number();
                        }

                        if (Peek() == ']')
                        {
                            AddSymbol(Peek(), LexemeType.RIGHT_BRACE);
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
                            AddSymbol(t, LexemeType.MINUS);
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
                            AddSymbol(t, LexemeType.SLASH);
                        }
                        break;


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
                    default:
                        //Lox.error(line, "Unexpected character.");
                        break;
                }
            }

            IsWhiteSpace();
        }

        internal void AddSymbol(char symbol, Int64 type)
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
                    lex.LexemeType |= LexemeType.INTERNALFUNCTION;
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
            Lexeme? identifier = new Lexeme(LexemeType.IDENTIFIER, this.line, this.column);
            
            identifier.AddToken(svalue);

            TryGetKeyWord(identifier);
            this.lexemes.Add(identifier);
        }
        internal void Number()
        {
            while(IsNumber(Peek()))
            {
                Advance();
            }

            var svalue = System.Text.Encoding.Default.GetString(Memcopy(fileData, start, current));
            Lexeme? number = new Lexeme(LexemeType.NUMBER, this.line, this.column);

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
            Lexeme? s = new Lexeme(LexemeType.STRING, this.line, this.column);
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

