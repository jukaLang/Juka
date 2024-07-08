using System.Text;

namespace JukaCompiler.Lexer
{
    internal class LexemeType
    {
        internal enum Types
        {
            UNDEFINED,
            CHAR,
            INTERNALFUNCTION,
            STRING,
            IDENTIFIER,
            NUMBER,
            WHITESPACE,
            SYMBOL,
            EOF,
            LEFT_PAREN,
            RIGHT_PAREN,
            LEFT_BRACE,
            RIGHT_BRACE,
            COMMA,
            DOT,
            MINUS,
            PLUS,
            SEMICOLON,
            SLASH,
            STAR,
            BANG,
            BANG_EQUAL,
            EQUAL,
            EQUAL_EQUAL,
            GREATER,
            GREATER_EQUAL,
            LESS,
            LESS_EQUAL,
            AND,
            CLASS,
            ELSE,
            FALSE,
            FUNC,
            SUB,
            FOR,
            IF,
            NULL,
            OR,
            PRINTLINE,
            RETURN,
            SUPER,
            THIS,
            TRUE,
            VAR,
            WHILE,
            INT,
            LONG,
            SHORT,
            DOUBLE,
            FLOAT,
            BOOL,
            PRINT,
            BREAK,
            ARRAY,
            PLUSPLUS,
            NEW,
            DELETE,
        }
    }
    internal class Lexeme
    {
        private StringBuilder tokenBuilder = new();
        private bool isKeyWord = false;
        private Int64 typeOfKeyWord;
        private int lineNumber;
        private int columnNumber;

        internal LexemeType.Types LexemeType { get; set; }

        internal Lexeme()
        {
        }

        internal Lexeme(LexemeType.Types ltype, int lineNumber, int columnNumber) : this()
        {
            this.LexemeType = ltype;
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        internal void AddToken(char token)
        {
            tokenBuilder.Append(token);
        }

        internal void AddToken(string token)
        {
            this.tokenBuilder.Append(token);
        }

        internal void AddToken(Lexeme? token)
        {
            this.tokenBuilder.Append(token?.ToString());
        }

        internal bool IsKeyWord
        {
            set { isKeyWord = true; }
            get { return isKeyWord; }
        }

        internal Int64 TypeOfKeyWord
        {
            get { return this.typeOfKeyWord; } 
            set { this.typeOfKeyWord = value;}
        }

        internal int LineNumber
        {
            get { return lineNumber; }
        }

        internal int ColumnNumber
        {
            get { return columnNumber; }
        }

        public override string ToString()
        {
            return tokenBuilder.ToString();
        }

        internal string Literal()
        {
            return ToString();
        }
    }
}
