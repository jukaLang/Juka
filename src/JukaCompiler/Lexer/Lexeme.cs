using System.Text;

namespace JukaCompiler.Lexer
{
    internal enum LexemeType
    {
        UNDEFINED           = 0x01,
        CHAR                = 0x02,
        INTERNALFUNCTION    = 0x04,

        // Literals.
        STRING              = 0x08,
        IDENTIFIER          = 0x10,
        NUMBER              = 0x20,
        WHITESPACE          = 0x40,
        SYMBOL              = 0x80,
        EOF                 = 0x100,

        // Single-character tokens.
        LEFT_PAREN          = 0x200,
        RIGHT_PAREN         = 0x400,
        LEFT_BRACE          = 0x800,
        RIGHT_BRACE         = 0x1000,
        COMMA               = 0x2000,
        DOT                 = 0x4000,
        MINUS               = 0x8000,
        PLUS                = 0x10000,
        SEMICOLON           = 0x20000,
        SLASH               = 0x400000,
        STAR                = 0x800000,

        // One or two character tokens.
        BANG                = 0x1000000,
        BANG_EQUAL          = 0x2000000,
        EQUAL               = 0x4000000,
        EQUAL_EQUAL         = 0x8000000,
        GREATER             = 0x10000000,
        GREATER_EQUAL       = 0x20000000,
        LESS                = 0x40000000,
        LESS_EQUAL          = 0x80000000,

        // Keywords.
        AND                 = 0x100000000,
        CLASS,
        ELSE,
        FALSE,
        FUNC,
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
        BOOL
    }

    internal class Lexeme
    {
        private StringBuilder tokenBuilder = new StringBuilder();
        private bool isKeyWord = false;
        private LexemeType typeOfKeyWord;
        internal LexemeType LexemeType { get; set; }

        internal Lexeme()
        {
        }

        internal Lexeme(LexemeType ltype) : this()
        {
            this.LexemeType = ltype;
        }

        internal void AddToken(char token)
        {
            tokenBuilder.Append(token);
        }

        internal bool IsKeyWord
        {
            set { isKeyWord = true; }
            get { return isKeyWord; }
        }

        internal LexemeType TypeOfKeyWord
        {
            get { return this.typeOfKeyWord; } 
            set { this.typeOfKeyWord = value;}
        }

        public override string ToString()
        {
            return tokenBuilder.ToString();
        }
    }
}
