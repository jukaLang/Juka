using JukaCompiler.Lexer;

namespace JukaCompiler.Expressions
{
    internal abstract class Expr
    {
        /// <summary>
        /// Interface for visiting different expression types
        /// </summary>
        internal interface IVisitor<R>
        {
            /// <summary>
            /// Visit an Assign expression
            /// </summary>
            R VisitAssignExpr(Assign expr);

            /// <summary>
            /// Visit a Binary expression
            /// </summary>
            R VisitBinaryExpr(Binary expr);

            /// <summary>
            /// Visit a Call expression
            /// </summary>
            R VisitCallExpr(Call expr);

            /// <summary>
            /// Visit a Get expression
            /// </summary>
            R VisitGetExpr(Get expr);

            /// <summary>
            /// Visit a Grouping expression
            /// </summary>
            R VisitGroupingExpr(Grouping expr);


            /// <summary>
            /// Visit a Literal expression
            /// </summary>
            R VisitLiteralExpr(Literal expr);

            /// <summary>
            /// Visit a Logical expression
            /// </summary>
            R VisitLogicalExpr(Logical expr);

            /// <summary>
            /// Visit a Set expression
            /// </summary>
            R VisitSetExpr(Set expr);

            /// <summary>
            /// Visit a Super expression
            /// </summary>
            R VisitSuperExpr(Super expr);

            /// <summary>
            /// Visit a This expression
            /// </summary>
            R VisitThisExpr(This expr);

            /// <summary>
            /// Visit an Unary expression
            /// </summary>
            R VisitUnaryExpr(Unary expr);

            /// <summary>
            /// Visit a Variable expression
            /// </summary>
            R VisitVariableExpr(Variable expr);

            /// <summary>
            /// Visit a LexemeTypeLiteral expression
            /// </summary>
            R VisitLexemeTypeLiteral(LexemeTypeLiteral expr);

            /// <summary>
            /// Visit an ArrayDeclaration expression
            /// </summary>
            R VisitArrayExpr(ArrayDeclarationExpr expr);

            /// <summary>
            /// Visit a NewDeclaration expression
            /// </summary>
            R VisitNewExpr(NewDeclarationExpr expr);

            /// <summary>
            /// Visit an ArrayAccess expression
            /// </summary>
            R VisitArrayAccessExpr(ArrayAccessExpr expr);

            /// <summary>
            /// Visit a DeleteDeclaration expression
            /// </summary>
            R VisitDeleteExpr(DeleteDeclarationExpr expr);
        }




        

        internal abstract R Accept<R>(IVisitor<R> visitor);

        private Lexeme? expressionLexeme;
        internal Lexeme? initializerContextVariableName;

        internal Lexeme? ExpressionLexeme
        {
            get => expressionLexeme;
            set { expressionLexeme = value; }
        }
        internal string ExpressionLexemeName
        {
            get => this.expressionLexeme?.ToString()!;
        }


        /// <summary>
        /// Represents an assignment expression in the abstract syntax tree
        /// </summary>
        internal class Assign : Expr
        {
            internal Expr value;


            /// <summary>
            /// Initializes a new instance of the Assign class with the provided expression lexeme and value
            /// </summary>
            internal Assign(Lexeme ExpressionLexeme, Expr value)
            {
                this.expressionLexeme = ExpressionLexeme;
                this.value = value;
            }


            /// <summary>
            /// Accepts a visitor and calls the VisitAssignExpr method on it
            /// </summary>
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

       /// <summary>
       /// This code defines a class Variable that inherits from Expr. It stores a LexemeType.Types value and 
       /// initializes it using the LexemeType of the provided ExpressionLexeme. 
       /// The class provides an Accept method that calls the VisitVariableExpr method on a visitor object.
       /// </summary>
        internal class Variable : Expr
        {
            internal LexemeType.Types lexemeType;

            internal Variable(Lexeme ExpressionLexeme)
            {
                this.ExpressionLexeme = ExpressionLexeme;
                this.lexemeType = ExpressionLexeme.LexemeType;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class Binary that extends Expr. It represents a binary expression with a left expression, an operator, and a right expression. The class has a constructor to initialize these values and an Accept method to allow visitors to process instances of Binary.
        /// </summary>
        internal class Binary : Expr
        {
            internal Expr? left;
            internal Lexeme? op;
            internal Expr? right;

            internal Binary(Expr expr, Lexeme op, Expr right)
            {
                this.left = expr;
                this.op = op;
                this.right = right;
            }

            internal Binary()
            {
                this.left=null;
                this.right=null;
                this.op=null;
            }

           // Accepts a visitor and calls its VisitBinaryExpr method with 'this' BinaryExpr object.
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }


        /// <summary>
        /// This code defines a class Increment that extends Expr. 
        /// It has two internal fields: variable and opLexeme, which are initialized through the constructor. The Accept method is overridden to throw a NotImplementedException.
        /// </summary>
        internal class Increment : Expr
        {
            internal Lexeme variable;
            internal Lexeme opLexeme;

            internal Increment(Lexeme variable, Lexeme opLexeme)
            {
                this.variable = variable;
                this.opLexeme = opLexeme;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This code defines a class NewDeclarationExpr that extends Expr. 
        /// It contains a property NewDeclarationExprInit and a constructor that initializes it. 
        /// The Accept method allows visitors to process instances of NewDeclarationExpr.
        /// </summary>
        internal class NewDeclarationExpr : Expr
        {
            public Expr NewDeclarationExprInit { get; }

            internal NewDeclarationExpr(Expr expr)
            {
                this.NewDeclarationExprInit = expr;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitNewExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class DeleteDeclarationExpr that extends Expr. 
        /// It has a public property variable of type Expr, a constructor to initialize the property, and an Accept method that allows visitors to process instances of DeleteDeclarationExpr 
        /// by calling VisitDeleteExpr on the visitor with this instance of the class.
        /// </summary>
        internal class DeleteDeclarationExpr : Expr
        {
            public Expr variable;

            internal DeleteDeclarationExpr(Expr expr)
            {
                this.variable = expr;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitDeleteExpr(this);
            }
        }



        /// <summary>
        /// This code defines a class ArrayDeclarationExpr that represents an array declaration expression. 
        /// It contains properties for the array size and name. The class has constructors to create instances with different parameters. 
        /// The Accept method allows visitors to process instances of ArrayDeclarationExpr.
        /// </summary>

        internal class ArrayDeclarationExpr : Expr
        {
            internal int ArraySize { get; }
            internal Lexeme ArrayDeclarationName { get; }

            internal ArrayDeclarationExpr(int size)
            {
                this.ArraySize = size;
                this.ArrayDeclarationName = new Lexeme(0,0,0);
            }

            internal ArrayDeclarationExpr(Lexeme arrayDeclarationName, Lexeme arraySize)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = arrayDeclarationName;
                this.ArrayDeclarationName = arrayDeclarationName;
            }

            // Overrides the Accept method from the base class Expr.
            // It accepts a visitor implementing the IVisitor interface and calls the VisitArrayExpr method on the visitor with this instance of ArrayExpr.
            // Returns the result of visiting the ArrayExpr.
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class ArrayAccessExpr that extends another class Expr. 
        /// It represents an expression involving array access. 
        /// The class has properties for ArrayIndex, ArrayVariableName, LvalueExpr, and HasInitializer. 
        /// It has two constructors, one with an initializer and one without. 
        /// The Accept method allows visitors to process instances of ArrayAccessExpr.
        /// </summary>
        internal class ArrayAccessExpr : Expr
        {
            internal int ArrayIndex { get; }
            internal Lexeme ArrayVariableName { get; }
            internal Expr LvalueExpr { get; }
            internal bool HasInitalizer { get; }

            internal ArrayAccessExpr(Lexeme arrayVariableName, Lexeme arraySize)
            {
                ArrayIndex = int.Parse(arraySize.ToString());
                ExpressionLexeme = ArrayVariableName = arrayVariableName;
                HasInitalizer = false;
                LvalueExpr = new DefaultExpr();
            }

            internal ArrayAccessExpr(Lexeme arrayVariableName, Lexeme arraySize, Expr expr)
            {
                ArrayIndex = int.Parse(arraySize.ToString());
                ExpressionLexeme = ArrayVariableName = arrayVariableName;
                LvalueExpr = expr;
                HasInitalizer = true;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayAccessExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class Call that extends Expr. It represents a function call expression with a callee, a list of arguments, and a flag indicating whether it is callable. 
        /// The Accept method is overridden to call the VisitCallExpr method on a visitor and return the result.
        /// </summary>
        internal class Call : Expr
        {
            internal Expr callee;
            internal List<Expr> arguments;
            internal bool isJukaCallable = false;

            internal Call(Expr callee, bool isCallable, List<Expr> arguments)
            {
                this.callee = callee;
                this.arguments = arguments;
                this.isJukaCallable = isCallable;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class Get that extends another class Expr. 
        /// It represents a get operation and accepts an expression and a Lexeme object. 
        /// The Accept method allows visitors to process instances of Get.
        /// </summary>
        internal class Get : Expr
        {
            internal Expr expr;
            internal Get(Expr expr, Lexeme lex)
            {
                this.expr = expr;
                this.ExpressionLexeme = lex;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class Unary that represents a unary expression in an abstract syntax tree. 
        /// It stores the lexeme type of the unary operation and provides a method to access this type. 
        /// The class implements the Accept method to allow visitors to process instances of Unary.
        /// </summary>
        internal class Unary : Expr
        {
            private LexemeType.Types lexemeType;
            internal Unary(Lexeme lex, LexemeType.Types lexemeType)
            {
                expressionLexeme = lex;
                this.lexemeType = lexemeType;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            public LexemeType.Types LexemeType => lexemeType;
        }

        /// <summary>
        /// This code defines a class Grouping that represents a grouping expression in an abstract syntax tree. 
        /// It can hold an inner expression (Expr) and implements the Accept method to allow visitors to process instances of Grouping.
        /// </summary>
        internal class Grouping : Expr
        {
            internal Expr? expression;

            internal Grouping(Expr expr)
            {
                expression = expr;
            }

            internal Grouping()
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        /// <summary>
        /// This code defines a class Literal that represents a literal expression in an abstract syntax tree. It stores the value and type of the literal along with the associated lexeme. The class implements the Accept method to allow visitors to process instances of Literal. 
        /// It also provides a method LiteralValue to get the value of the literal, and a property Type to access the type of the literal.
        /// </summary>
        internal class Literal : Expr
        {
            private object? value;
            private LexemeType.Types type;

            internal Literal(Lexeme literal, LexemeType.Types type)
            {
                ExpressionLexeme = literal;
                value = literal;
                this.type = type;
            }
            internal Literal()
            {
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            internal object? LiteralValue()
            {
                if (value == null)
                {
                    throw new ArgumentNullException("literal");
                }

                LexemeTypeLiteral? v = new()
                {
                    lexemeType = Type,
                    literal = value.ToString()
                };

                return v;
                //return literal;
            }

            internal LexemeType.Types Type
            {
                get { return type; }
            }
        }
        /// <summary>
        /// This code defines a class Logical that extends Expr. 
        /// It has a constructor that takes an Expr, a Lexeme, and another Expr as parameters. 
        /// The Accept method throws a NotImplementedException.
        /// </summary>
        internal class Logical : Expr
        {
            internal Logical(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This code defines a class Set that extends Expr. It has a constructor that takes three parameters: an Expr, a Lexeme, and another Expr. 
        /// The Accept method is overridden to throw a NotImplementedException.
        /// </summary>
        internal class Set : Expr
        {
            internal Set(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// This code defines a class named Super that extends the Expr class. 
        /// It has a constructor that takes three parameters: an Expr, a Lexeme, and another Expr. 
        /// The Accept method is overridden to throw a NotImplementedException.
        /// </summary>
        internal class Super : Expr
        {
            internal Super(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This code defines a class This that extends Expr. 
        /// It has a constructor that takes an Expr, a Lexeme, and another Expr as parameters. The Accept method throws a NotImplementedException.
        /// </summary>
        internal class This : Expr
        {
            internal This(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class DefaultExpr : Expr
        {
            internal DefaultExpr()
            { }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// This code defines a class LexemeTypeLiteral that extends Expr. It contains fields to store a LexemeType.Types and an object.
        /// It has properties Literal and LexemeType to access and set these fields. The Accept method allows visitors to process instances of LexemeTypeLiteral.
        /// </summary>
        internal class LexemeTypeLiteral : Expr
        {
            internal LexemeType.Types lexemeType;
            internal object? literal;

            internal new object Literal
            {
                get => literal ?? string.Empty;
                set => literal = value;
            }

            internal LexemeType.Types LexemeType
            {
                get => lexemeType;
                set => lexemeType = value;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLexemeTypeLiteral(this);
            } 
        }
    }
}
