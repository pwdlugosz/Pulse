parser grammar PulseParser;

options
{
	tokenVocab = PulseLexer;
}

compileUnit
	: expression	
	| matrix_expression
	| table_expression
	| (action_expression SEMI_COLON)+
	| EOF
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- ACTIONS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
action_expression
	: type var_name (ASSIGN expression)? 													# DeclareScalar
	| type var_name LBRAC expression (COMMA expression)? RBRAC 								# DeclareMatrix1
	| type var_name LBRAC RBRAC ASSIGN matrix_expression 									# DeclareMatrix2
	| K_TABLE var_name (ASSIGN table_expression)? 											# DeclareTable

	| var_name ASSIGN var_name 																# ActionAssignVariable
	| var_name PLUS ASSIGN var_name 														# ActionIncermentVariable
	| var_name assignment expression 														# ActionScalarAssign
	| var_name increment 																	# ActionScalarIncrement
	| var_name LBRAC expression COMMA expression RBRAC assignment expression 				# ActionMatrixUnit2DAssign
	| var_name LBRAC expression COMMA expression RBRAC increment 							# ActionMatrixUnit2DIncrement
	| var_name LBRAC expression RBRAC assignment expression 								# ActionMatrixUnit1DAssign
	| var_name LBRAC expression RBRAC increment 											# ActionMatrixUnit1DIncrement
	| var_name LBRAC RBRAC assignment expression 											# ActionMatrixUnitAllAssign
	| var_name LBRAC RBRAC increment 														# ActionMatrixUnitAllIncrement
	| var_name LBRAC RBRAC assignment matrix_expression 									# ActionMatrixAssign
	| var_name ASSIGN table_expression 														# ActionTableAssign
	| var_name PLUS ASSIGN table_expression													# ActionTableIncrement1
	| var_name PLUS ASSIGN expression_or_wildcard_set 										# ActionTableIncrement2		
	
	| K_PRINT var_name (K_TO expression)? 													# ActionPrintVariable
	| K_PRINT table_expression (K_TO expression)? 											# ActionPrintTable
	| K_PRINT matrix_expression (K_TO expression)? 											# ActionPrintMatrix
	| K_PRINT expression_or_wildcard_set (K_TO expression)? 								# ActionPrintExpression

	| var_name LPAREN (parameter (COMMA parameter)?)? RPAREN								# ActionCallSeq
	| var_name LPAREN (parameter_name (COMMA parameter_name)?)? RPAREN						# ActionCallNamed
	
	/*
		both <set> {} and <do> DO {}; are nested actions but only DO can be used in alone: <set> must be a child in an expression tree;
	*/
	| LCURL (action_expression SEMI_COLON)+ RCURL																								# ActionSet				
	| K_DO LCURL (action_expression SEMI_COLON)+ RCURL																							# ActionDo
	
	| K_FOR_EACH IDENTIFIER K_IN table_expression LCURL (action_expression SEMI_COLON)+	RCURL													# ActionForEach	
	| K_FOR LPAREN (type)? var_name ASSIGN expression SEMI_COLON expression SEMI_COLON action_expression RPAREN 
		LCURL (action_expression SEMI_COLON)+ RCURL 																							# ActionFor
	| K_WHILE LPAREN expression RPAREN LCURL (action_expression SEMI_COLON)+ RCURL																# ActionWhile
	| K_IF LPAREN expression RPAREN action_expression (K_ELSE K_IF LPAREN expression RPAREN action_expression)* (K_ELSE action_expression)?		# ActionIF
	;

sub_if : K_IF LCURL action_expression+ RCURL;

parameter_name : PARAM lib_name ASSIGN parameter;
parameter 
	: var_name
	| table_expression
	| matrix_expression
	| LCURL expression_or_wildcard_set RCURL
	| expression
	;

// Compound Opperators //
assignment : (ASSIGN | PLUS ASSIGN | MINUS ASSIGN | MUL ASSIGN | DIV ASSIGN | DIV2 ASSIGN | MOD ASSIGN);
increment : (PLUS PLUS | MINUS MINUS);

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- TABLES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
table_expression
	: (K_JOIN | K_ALEFT_JOIN | K_LEFT_JOIN) LPAREN table_expression COMMA table_expression 
		COMMA t_join_on (K_AND t_join_on)* RPAREN DOT t_select (DOT where_clause)? tmod_distinct? tmod_order?							# TableExpressionJoin
	| table_expression DOT t_fold (DOT where_clause)? (DOT t_select)? tmod_distinct? tmod_order?										# TableExpressionFold
	| table_expression DOT t_select (DOT where_clause)? tmod_distinct? tmod_order?														# TableExpressionSelect
	| K_UNION LPAREN table_expression (COMMA table_expression)* RPAREN tmod_distinct? tmod_order?										# TableExpressionUnion
	| IDENTIFIER DOT IDENTIFIER																											# TableExpressionLookup
	| LCURL expression_or_wildcard_set (PIPE expression_or_wildcard_set)* RCURL															# TableExpressionLiteral
	| LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL																				# TableExpressionShell
	| table_expression DOT t_alias																										# TableExpressionAlias
	| LPAREN table_expression RPAREN																									# TableExpressionParens
	;

t_fold : K_GROUP LPAREN expression_or_wildcard_set RPAREN DOT K_FOLD LPAREN beta_reduction_list RPAREN (DOT t_alias)?;
t_select : K_SELECT LPAREN expression_or_wildcard_set RPAREN;
t_join_on : IDENTIFIER DOT IDENTIFIER EQ IDENTIFIER DOT IDENTIFIER;
t_alias : K_AS LPAREN IDENTIFIER RPAREN;
tmod_distinct : DOT K_DISTINCT LPAREN RPAREN;
tmod_order : DOT K_ORDER LPAREN LITERAL_INT (COMMA LITERAL_INT)* RPAREN;


// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- MATRIX ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //

 matrix_expression
	: NOT matrix_expression																# MatrixInvert
	| TILDA matrix_expression															# MatrixTranspose
	| matrix_expression MUL MUL matrix_expression										# MatrixTrueMul

	| matrix_expression op=(MUL | DIV | DIV2 | MOD) matrix_expression					# MatrixMulDiv
	| matrix_expression op=(MUL | DIV | DIV2 | MOD) expression							# MatrixMulDivLeft
	| expression op=(MUL | DIV | DIV2 | MOD) matrix_expression							# MatrixMulDivRight

	| matrix_expression op=(PLUS | MINUS) matrix_expression								# MatrixAddSub
	| expression op=(PLUS | MINUS) matrix_expression									# MatrixAddSubLeft
	| matrix_expression op=(PLUS | MINUS) expression									# MatrixAddSubRight

	| IDENTIFIER LBRAC RBRAC															# MatrixNakedLookup
	| IDENTIFIER DOT IDENTIFIER LBRAC RBRAC												# MatrixLookup
	| matrix_literal																	# MatrixLiteral
	| K_IDENTITY LPAREN expression COMMA type RPAREN									# MatrixIdent

	| LPAREN matrix_expression RPAREN													# MatrixParen
	;

matrix_literal 
	: vector_literal (COMMA vector_literal)*
	;
vector_literal 
	: LCURL expression (COMMA expression)* RCURL
	;

// ----------------------------------------------------------------------------------------------------- //
// --------------------------------------------- AGGREGATES -------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //

 beta_reduction_list 
	: beta_reduction (COMMA beta_reduction)*
	;
 beta_reduction
	: SET_REDUCTIONS LPAREN (expression_or_wildcard_set)? RPAREN (where_clause)? (K_AS IDENTIFIER)?
	;

// ----------------------------------------------------------------------------------------------------- //
// --------------------------------------------- EXPRESSIONS ------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //

// Sort helper //
sort_unit : expression (K_ASC | K_DESC)?;

 // Return Expression // 
expression_or_wildcard_set 
	: expression_or_wildcard (COMMA expression_or_wildcard)*
	;
expression_or_wildcard
	: expression_alias
	| IDENTIFIER DOT MUL
	; 

// Where Clause //
where_clause 
	: K_WHERE LPAREN expression RPAREN
	;

// Expressions + Alias //
expression_alias 
	: expression (K_AS IDENTIFIER)?
	;

// Expressions
expression
	: IDENTIFIER DOT type																				# Pointer // X.STRING.5
	| op=(NOT | PLUS | MINUS) expression																# Uniary
	| expression POW expression																			# Power
	| expression op=(MUL | DIV | MOD | DIV2) expression													# MultDivMod
	| expression op=(PLUS | MINUS) expression															# AddSub
	| expression op=(GT | GTE | LT | LTE) expression													# GreaterLesser
	| expression op=(EQ | NEQ) expression																# Equality
	| expression AND expression																			# LogicalAnd
	| expression op=(OR | XOR) expression																# LogicalOr
	
	| IDENTIFIER																						# NakedVariable
	| IDENTIFIER DOT IDENTIFIER																			# SpecificVariable
	| LITERAL_BOOL																						# LiteralBool
	| LITERAL_INT																						# LiteralInt
	| LITERAL_DOUBLE																					# LiteralDouble
	| LITERAL_DATE																						# LiteralDate
	| LITERAL_STRING																					# LiteralString
	| LITERAL_BLOB																						# LiteralBLOB
	| LITERAL_NULL																						# LiteralNull																
	| type																								# ExpressionType
	
	| expression NULL_OP expression																		# IfNullOp
	| expression IF_OP expression (ELSE_OP expression)?													# IfOp
	| LPAREN type RPAREN expression																		# Cast
	| IDENTIFIER LPAREN ( expression ( COMMA expression )* )? RPAREN									# BaseFunction
	| var_name LPAREN ( expression ( COMMA expression )* )? RPAREN										# LibraryFunction

	| var_name LBRAC expression COMMA expression RBRAC													# Matrix2D
	| var_name LBRAC expression RBRAC																	# Matrix1D
	| IDENTIFIER LBRAC expression COMMA expression RBRAC												# Matrix2DNaked
	| IDENTIFIER LBRAC expression RBRAC																	# Matrix1DNaked

	| LPAREN expression RPAREN																			# Parens
	;

// Library //
var_name : (lib_name DOT)? IDENTIFIER;
lib_name : IDENTIFIER | K_TABLE | T_BLOB | T_BOOL | T_DATE | T_DOUBLE | T_INT | T_STRING;

// Table logic //  
table_name 
	: IDENTIFIER DOT IDENTIFIER
	;

// Types //
type : (T_BLOB | T_BOOL | T_DATE | T_DOUBLE | T_INT | T_STRING) (DOT LITERAL_INT)?;