grammar DreamGrammar;

/*
 * Parser Rules
 */

compileUnit
	: classifications*
    ;

classifications
    : functionDeclaration
    | userDefinedTypeDecl
    ;

statement
    : ( expression )
	;

endLine
    : semiColon
    ;

semiColon
    : ';'
    ;

expression
	: variableDeclarationAssignment endLine
    | ifExpr
    | whileExpression
    | doWhileExpr
    | primitives endLine
    | functionCall endLine
    | userDefinedTypeFunctionReference endLine
    | userDefinedTypeVariableReference endLine
    | returnValue endLine
    ;

functionCall
    : funcName '()'
    | funcName (leftParen)(WS)*(rightParen)
    | funcName (leftParen) singleExpression (',' singleExpression)? (rightParen)
    ;

functionDeclaration
    : 'function' funcName '()' equalsign '{' (statement)* '}'
    | 'function' funcName leftParen parameterVariableDeclaration (',' parameterVariableDeclaration)? rightParen equalsign '{' (statement)* '}'
    ;

funcName
    : ID
    ;

returnValue
    : 'return'
    | 'return' types
    | 'return' (variable)
    | 'return' (functionCall)
    ;

leftParen
    : '('
    ;

rightParen
    : ')'
    ;

booleanExpression
    : ( BOOLEAN | variable | singleExpression ) (comparisonOperator) ( variable | primitiveTypes | singleExpression | userDefinedTypeVariableReference | userDefinedTypeFunctionReference )
    ;

primitives
    : 'sysExec' '(' STRING ')'
    | 'sysExec' '(' variable ')'
    ;

userDefinedTypeDecl
    : userDefinedTypeKeyWord userDefinedTypeName equalsign '{' (statement)* (functionDeclaration)* '}'
    ;

userDefinedTypeKeyWord
    : 'class'
    ;

userDefinedTypeName
    : ID
    ;

variable
    : ID
    ;

variableDeclaration
    : keywords identifierName
    | userDefinedTypeVariableDecl
    ;

parameterVariableDeclaration
    : keywords ID
    | userDefinedTypeVariableDecl
    ;

userDefinedTypeVariableDecl
    : userDefinedTypeName variable
    | userDefinedTypeName ID equalsign (New userDefinedTypeName)
    ;


userDefinedTypeVariableReference
    : userDefinedTypeName(userDefinedTypeResolutionOperator)variable
    ;

userDefinedTypeFunctionReference
    : userDefinedTypeName(userDefinedTypeResolutionOperator)functionCall
    ;

multiplication
    : '*'
    ;

breakKeyWord
    : Break
    ;
	
division
    : '/'
    ;

modulo
    :'%'
    ;

equalsign
    : '='
    ;

types
    : primitiveTypes
    ;

primitiveTypes
    : INT
	/*
	| FLOAT
	| DOUBLE
	| LONG
    | STRING
    | BOOLEAN
    | NULL
	*/
    ;

numericTypes
    : INT
	/*
    | FLOAT
    | DOUBLE
    | LONG
	*/
    ;

comparisonOperator
    : equalequal
    | lessthan
    | greaterthan
    | lessthanorequalto
    | greaterthanorequalto
    | booleanAndOperator
    | booleanOrOperator
    | notOperator
    ;

addSubtractOp
	: '+' | '-';

multiplyDivideOp
	: '*' | '/';

binaryOperator
    : multiplyDivideOp
	| addSubtractOp
	| equalsign
    ;

bitWiseOperators
    : bitAnd
    | bitOr
    | bitXor
    | bitNot
    | bitLeftShift
    | bitRigthShift
    ;

equalequal          : '==' ;
lessthan            : '<'  ;
greaterthan         : '>'  ;
lessthanorequalto   : '<=' ;
greaterthanorequalto: '>=' ;
spaceshipOperator	: '<=>';
booleanAndOperator  : '&&' ;
booleanOrOperator   : '||' ;
notOperator         : '!'  ;
bitAnd              : '&'  ;
bitOr               : '|'  ;
bitXor              : '^'  ;
bitNot              : '~'  ;
bitLeftShift        : '<<' ;
bitRigthShift       : '>>' ;

Break      : 'break';
Do         : 'do';
Instanceof : 'instanceof';
Typeof     : 'typeof';
Case       : 'case';
Else       : 'else';
New        : 'new';
Var        : 'var';
Catch      : 'catch';
Finally    : 'finally';
Return     : 'return';
Void       : 'void';
Continue   : 'continue';
For        : 'for';
Switch     : 'switch';
While      : 'while';
Debugger   : 'debugger';
Function   : 'function';
This       : 'this';
With       : 'with';
Default    : 'default';
If         : 'if';
Throw      : 'throw';
Delete     : 'delete';
In         : 'in';
Try        : 'try';

keywords
    : 'int'
    | 'float'
    | 'double'
    | 'long'
    | 'object'
    | 'boolean'
    | 'string'
    | 'class'
    ;

ifExpr
    : If '(' ( binaryExpression )+ ')' '{' ( statement )* '}'
    ;


whileExpression
    : While '(' ( combinedComparisonExpressions )+ ')' '{' ( statement )* '}'
    ;

doWhileExpr
    : Do '{' ( statement )* '}' While '(' ( combinedComparisonExpressions )+ ')'
    ;


userDefinedTypeResolutionOperator
    : '::'
    ;

expressionSequence
 : singleExpression ( ',' singleExpression )*
 ;

 identifierName :
     ID
     ;

	 /*
singleExpression
 : Delete singleExpression                                                  # DeleteExpression
 | Void singleExpression                                                    # VoidExpression
 | Typeof singleExpression                                                  # TypeofExpression
 | singleExpression '[' expressionSequence ']'                              # MemberIndexExpression
 | singleExpression '.' identifierName                                      # MemberDotExpression
 | '++' singleExpression                                                    # PreIncrementExpression
 | '--' singleExpression                                                    # PreDecreaseExpression
 | '+' singleExpression                                                     # UnaryPlusExpression
 | '-' singleExpression                                                     # UnaryMinusExpression
 | '~' singleExpression                                                     # BitNotExpression
 | notOperator singleExpression                                             # NotExpression
 | singleExpression bitWiseOperators singleExpression                       # BitAndExpression
 | singleExpression ( bitLeftShift | bitRigthShift ) singleExpression       # BitShiftExpression
 | singleExpression Instanceof singleExpression                             # InstanceofExpression
 | singleExpression In singleExpression                                     # InExpression
 | singleExpression '?' singleExpression ':' singleExpression               # TernaryExpression
 | This                                                                     # ThisExpression
 | literal                                                                  # LiteralExpression
 | variable                                                                 # VariableExpression
 | functionCall                                                             # FunctionCallExpression
 | '(' expressionSequence ')'                                               # ParenthesizedExpression
 | DecimalLiteral														    # DecimalValue
 | INT                                                                      # IntValue
 | STRING                                                                   # StringValue
 | variableDeclarationExpression										    # VBD
 ;
 */


variableExpressions
 : singleExpression
;

 variableDeclarationAssignment
 : keywords variable assignmentOperator combinedExpressions
 ; 


 binaryExpression
 : singleExpression binaryOperator singleExpression
 ;

  combinedExpressions
 : (leftParen)* singleExpression (rightParen)*
 | (leftParen)* singleExpression (binaryOpAndSingleExpression)+ (rightParen)*
 | (leftParen)* singleExpression (binaryOperator binaryOpAndDoubleExpression)+ (rightParen)*
 | (binaryOpAndDoubleExpression)+ (binaryOpAndSingleExpression)*
 | (binaryOpAndDoubleExpression)+ (binaryOperator binaryOpAndDoubleExpression)?
 ;

 binaryOpAndSingleExpression
 : (leftParen)* binaryOperator singleExpression (rightParen)*
 ;

 binaryOpAndDoubleExpression
 : (leftParen)* singleExpression binaryOpAndSingleExpression (rightParen)*
 ;


 combinedComparisonExpressions
 : (leftParen)* singleExpression (rightParen)*
 | (leftParen)* singleExpression (binaryOpAndSingleComparisonExpression)+ (rightParen)*
 | (leftParen)* singleExpression (comparisonOperator binaryOpAndDoubleComparisonExpression)+ (rightParen)*
 | (binaryOpAndDoubleComparisonExpression)+ (binaryOpAndSingleComparisonExpression)*
 | (binaryOpAndDoubleComparisonExpression)+ (binaryOperator binaryOpAndDoubleComparisonExpression)?
 ;

 binaryOpAndDoubleComparisonExpression
 : (leftParen)* singleExpression binaryOpAndSingleComparisonExpression (rightParen)*
 ;

  binaryOpAndSingleComparisonExpression
 : (leftParen)* comparisonOperator singleExpression (rightParen)*
 ;



singleExpression
 : literal																	# LiteralExpression
 | variable                                                                 # VariableExpression
 | functionCall                                                             # FunctionCallExpression
 | INT                                                                      # IntValue
 | STRING                                                                   # StringValue
 | variableDeclarationAssignment											# VariableDeclarationExpression
 ;

evaluatableExpression : singleExpression                               # Evaluatable ;

 assignmentExpression
     : variableDeclaration equalsign ( singleExpression | variable | functionCall | primitiveTypes | booleanExpression | userDefinedTypeVariableReference | userDefinedTypeVariableReference)
     | userDefinedTypeVariableReference equalsign ( variable | primitiveTypes)
 	;

 reassignmentExpression
     : variable equalsign singleExpression 
	;


assignmentOperator
 : '=' 
 | '*=' singleExpression
 | '/=' singleExpression
 | '%=' singleExpression
 | '+=' singleExpression
 | '-=' singleExpression
 | '<<=' singleExpression 
 | '>>=' singleExpression
 | '>>>=' singleExpression
 | '&=' singleExpression
 | '^=' singleExpression
 | '|=' singleExpression
 ;

 literal
 : ( BOOLEAN | NULL )
 ;

/*
 literal
  : ( NullLiteral
    | BooleanLiteral
    | StringLiteral
    | RegularExpressionLiteral
    )
  | numericLiteral
  ;

/// 7.8.3 Numeric Literals
HexIntegerLiteral
 : '0' [xX] HexDigit+
 ;

OctalIntegerLiteral
 : {!this.strictMode}? '0' OctalDigit+
 ;
 */


/*
 * Lexer Rules
 */

/// 7.8.3 Numeric Literals
/*
DecimalLiteral
 : DecimalIntegerLiteral '.' DecimalDigit* ExponentPart?
 | '.' DecimalDigit+ ExponentPart?
 | DecimalIntegerLiteral ExponentPart?
 ;
*/

NULL
    :'null'
    ;

BOOLEAN
    : 'true'
    |'false'
    ;

INT
    : [0-9]+
    ;

/*
FLOAT
    : ('0'..'9')+ '.' ('0'..'9')*
    ;

DOUBLE
    : ('0'..'9')+ '.' ('0'..'9')*
    ;

LONG
    : ('0'..'9')+ '.' ('0'..'9')*
    ;
*/

ESC_CHARS
    : '\\' ('\\"'|'\\'|'/'|'b'|'f'|'n'|'r'|'t')
    | '\\' 'u' [0-9a-fA-F] [0-9a-fA-F] [0-9a-fA-F] [0-9a-fA-F]
    ;

STRING
    :  '"' ( ESC_CHARS | ~('\\'|'"') )* '"' |'\'' ( ESC_CHARS  | ~('\\'|'\'') )* '\''  |'`' (ESC_CHARS | ~('\\'| '`') )* '`'
    ;

ID
    : ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_'| '-' )*
    ;

WS
    : [ \t\r\n]+ -> skip
    ;

COMMENT
    : '/*' (COMMENT|.)*? '*/' -> channel(HIDDEN)
    ;

LINE_COMMENT
    : '//' .*? '\n' -> channel(HIDDEN)
    ;
 /*
StringLiteral
 : '"' DoubleStringCharacter* '"'
 | '\'' SingleStringCharacter* '\''
 ;


 fragment DoubleStringCharacter
  : ~["\\\r\n]
  | '\\' EscapeSequence
  | LineContinuation
  ;
 fragment SingleStringCharacter
  : ~['\\\r\n]
  | '\\' EscapeSequence
  | LineContinuation
  ;
 fragment EscapeSequence
  : CharacterEscapeSequence
  | '0' // no digit ahead! TODO
  | HexEscapeSequence
  | UnicodeEscapeSequence
  ;
 fragment CharacterEscapeSequence
  : SingleEscapeCharacter
  | NonEscapeCharacter
  ;
 fragment HexEscapeSequence
  : 'x' HexDigit HexDigit
  ;
 fragment UnicodeEscapeSequence
  : 'u' HexDigit HexDigit HexDigit HexDigit
  ;
 fragment SingleEscapeCharacter
  : ['"\\bfnrtv]
  ;
  */

  fragment LineContinuation
   : '\\' LineTerminatorSequence
   ;

fragment LineTerminatorSequence
 : '\r\n'
 | LineTerminator
 ;

 LineTerminator
  : [\r\n\u2028\u2029] -> channel(HIDDEN)
  ;

  fragment NonEscapeCharacter
   : ~['"\\bfnrtv0-9xu\r\n]
   ;

/*
fragment DecimalDigit
 : [0-9]
 ;
fragment HexDigit
 : [0-9a-fA-F]
 ;
fragment OctalDigit
 : [0-7]
 ;
fragment DecimalIntegerLiteral
 : '0'
 | [1-9] DecimalDigit*
 ;
fragment ExponentPart
 : [eE] [+-]? DecimalDigit+
 ;
 */

 /*
 : Function Identifier? '(' formalParameterList? ')' '{' functionBody '}' # FunctionExpression
 | singleExpression arguments                                             # ArgumentsExpression
 | New singleExpression arguments?                                        # NewExpression
 : singleExpression {!this.here(Visitor.LineTerminator)}? '++'   # PostIncrementExpression
 | singleExpression {!this.here(Visitor.LineTerminator)}? '--'   # PostDecreaseExpression
 */
