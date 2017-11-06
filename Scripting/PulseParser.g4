parser grammar PulseParser;

options
{
	tokenVocab = PulseLexer;
}

compileUnit
	: scalar_expression	
	| matrix_expression
	| table_expression
	| record_expression
	| (action_expression SEMI_COLON)+
	| EOF
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- ACTIONS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
action_expression
	: type var_name (ASSIGN scalar_expression)? 															# DeclareScalar
	| K_MATRIX matrix_name ASSIGN matrix_expression 														# DeclareMatrix
	| K_RECORD record_name ASSIGN record_expression															# DeclareRecord
	| K_TABLE table_name ASSIGN table_expression															# DeclareTable
	| K_TABLE table_name ASSIGN LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL						# CTORTable

	| var_name assignment scalar_expression 																# ActionScalarAssign
	| var_name increment 																					# ActionScalarIncrement
	| var_name LBRAC scalar_expression COMMA scalar_expression RBRAC assignment scalar_expression 			# ActionMatrixUnit2DAssign
	| var_name LBRAC scalar_expression COMMA scalar_expression RBRAC increment 								# ActionMatrixUnit2DIncrement
	| var_name LBRAC scalar_expression RBRAC assignment scalar_expression 									# ActionMatrixUnit1DAssign
	| var_name LBRAC scalar_expression RBRAC increment 														# ActionMatrixUnit1DIncrement

	| matrix_name assignment scalar_expression 																# ActionMatrixUnitAllAssign
	| matrix_name increment 																				# ActionMatrixUnitAllIncrement
	| matrix_name assignment matrix_expression 																# ActionMatrixAssign

	| record_name ASSIGN record_expression																	# ActionRecordAssign

	| table_name ASSIGN table_expression 																	# ActionTableAssign
	| table_name PLUS ASSIGN table_expression																# ActionTableIncrement1
	| table_name PLUS ASSIGN record_expression																# ActionTableIncrement2		
	
	| K_PRINT scalar_expression (K_TO scalar_expression)? 													# ActionPrintScalar
	| K_PRINT matrix_expression (K_TO scalar_expression)? 													# ActionPrintMatrix
	| K_PRINT record_expression (K_TO scalar_expression)? 													# ActionPrintRecord
	| K_PRINT table_expression (K_TO scalar_expression)? 													# ActionPrintTable

	| var_name LPAREN (parameter (COMMA parameter)*)? RPAREN												# ActionCallSeq
	| var_name LPAREN (parameter_name (COMMA parameter_name)*)? RPAREN										# ActionCallNamed
	
	/*
		both <set> {} and <do> DO {}; are nested actions but only DO can be used in alone: <set> must be a child in an scalar_expression tree;
	*/
	| LCURL (action_expression SEMI_COLON)+ RCURL																												# ActionSet				
	| K_DO LCURL (action_expression SEMI_COLON)+ RCURL																											# ActionDo
	
	| K_FOR_EACH IDENTIFIER K_IN table_expression LCURL (action_expression SEMI_COLON)+	RCURL																	# ActionForEach	
	| K_FOR LPAREN (type)? var_name ASSIGN scalar_expression SEMI_COLON scalar_expression SEMI_COLON action_expression RPAREN 
		LCURL (action_expression SEMI_COLON)+ RCURL 																											# ActionFor
	| K_WHILE LPAREN scalar_expression RPAREN LCURL (action_expression SEMI_COLON)+ RCURL																		# ActionWhile
	| K_IF LPAREN scalar_expression RPAREN action_expression (K_ELSE K_IF LPAREN scalar_expression RPAREN action_expression)* (K_ELSE action_expression)?		# ActionIF
	;

sub_if : K_IF LCURL action_expression+ RCURL;

parameter_name : PARAM lib_name ASSIGN parameter;
parameter 
	: var_name
	| table_expression
	| matrix_expression
	| record_expression
	| scalar_expression
	;

// Compound Opperators //
assignment : (ASSIGN | PLUS ASSIGN | MINUS ASSIGN | MUL ASSIGN | DIV ASSIGN | DIV2 ASSIGN | MOD ASSIGN);
increment : (PLUS PLUS | MINUS MINUS);

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- TABLES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
table_expression
	: table_expression (K_JOIN | K_LEFT_JOIN | K_ALEFT_JOIN) table_expression 
		K_ON t_join_on_set record_expression (where_clause)? (tmod_distinct)? (tmod_order)?						# TableExpressionJoin
	| table_expression t_fold (record_expression)? (where_clause)? (tmod_distinct)? (tmod_order)?				# TableExpressionFold
	| table_expression record_expression (where_clause)? (tmod_distinct)? (tmod_order)?							# TableExpressionSelect
	| table_expression (PLUS table_expression)+ (tmod_distinct)? (tmod_order)?									# TableExpressionUnion
	| table_name																								# TableExpressionLookup
	| LCURL record_chain RCURL												 									# TableExpressionLiteral
	//| LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL														# TableExpressionCTOR
	| table_expression t_alias																					# TableExpressionAlias
	| LPAREN table_expression RPAREN																			# TableExpressionParens
	;

t_fold : DIV record_expression MOD GT beta_reduction_list LT t_alias?;					// / < rex > % < agg > : A
t_join_on_set : LPAREN t_join_on (K_AND t_join_on)* RPAREN; 
t_join_on : IDENTIFIER DOT IDENTIFIER EQ IDENTIFIER DOT IDENTIFIER;
t_alias : K_AS IDENTIFIER;
tmod_distinct : K_DISTINCT;
tmod_order : K_ORDER LPAREN LITERAL_INT (COMMA LITERAL_INT)* RPAREN;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- MATRIX ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
 matrix_expression
	: NOT matrix_expression																	# MatrixInvert
	| TILDA matrix_expression																# MatrixTranspose
	| matrix_expression MUL MUL matrix_expression											# MatrixTrueMul
	| matrix_expression op=(MUL | DIV | DIV2 | MOD) matrix_expression						# MatrixMulDiv
	//| matrix_expression op=(MUL | DIV | DIV2 | MOD) scalar_expression						# MatrixMulDivLeft
	//| scalar_expression op=(MUL | DIV | DIV2 | MOD) matrix_expression						# MatrixMulDivRight
	| matrix_expression op=(PLUS | MINUS) matrix_expression									# MatrixAddSub
	//| scalar_expression op=(PLUS | MINUS) matrix_expression								# MatrixAddSubLeft
	//| matrix_expression op=(PLUS | MINUS) scalar_expression								# MatrixAddSubRight
	| matrix_name																			# MatrixLookup
	| LBRAC record_chain RBRAC																# MatrixLiteral
	| type LBRAC scalar_expression (COMMA scalar_expression)? RBRAC							# MatrixCTOR
	| LPAREN matrix_expression RPAREN														# MatrixParen
	;

// ----------------------------------------------------------------------------------------------------- //
// --------------------------------------------- AGGREGATES -------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
 beta_reduction_list 
	: beta_reduction (COMMA beta_reduction)*
	;
 beta_reduction
	: SET_REDUCTIONS LPAREN (naked_record)? RPAREN (where_clause)? (K_AS IDENTIFIER)?
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- RECORDS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
record_chain 
	: record_expression (COMMA record_expression)*
	;

record_expression
	: LT naked_record GT														# RecordExpressionLiteral			// < scalar, ... , scalar >
	| record_name																# RecordExpressionLookup			// r<>
	| record_expression (MUL record_expression)+								# RecordExpressionUnion				// r<> + s<> + t<>
	| LPAREN record_expression RPAREN											# RecordExpressionParens			// (r<>)
	;

naked_record
	: scalar_expression_alias (COMMA scalar_expression_alias)+
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- SCALARS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //

// Expressions + Alias //
scalar_expression_alias 
	: scalar_expression (K_AS IDENTIFIER)?
	;

// Where Clause //
where_clause 
	: K_WHERE scalar_expression
	;

// Expressions
scalar_expression
	: IDENTIFIER DOT type																				# Pointer			// X.STRING.5
	| op=(NOT | PLUS | MINUS) scalar_expression															# Uniary			// -X
	| scalar_expression POW scalar_expression															# Power				// X ^ Y
	| scalar_expression op=(MUL | DIV | MOD | DIV2) scalar_expression									# MultDivMod		// X / Y
	| scalar_expression op=(PLUS | MINUS) scalar_expression												# AddSub			// X + Y
	| scalar_expression op=(GT | GTE | LT | LTE) scalar_expression										# GreaterLesser		// X < Y
	| scalar_expression op=(EQ | NEQ) scalar_expression													# Equality			// X == Y
	| scalar_expression AND scalar_expression															# LogicalAnd		// X && Y
	| scalar_expression op=(OR | XOR) scalar_expression													# LogicalOr			// X || Y
	
	| var_name																							# TableOrScalarMember	// X.Name
	| record_expression LPAREN IDENTIFIER RPAREN														# RecordMember			// R<>.Name
	| matrix_expression LPAREN scalar_expression (COMMA scalar_expression)? RPAREN						# MatrixMember			// M[]()

	| LITERAL_STRING																					# LiteralString
	| LITERAL_TEXT																						# LiteralText
	| LITERAL_BLOB																						# LiteralBLOB
	| LITERAL_DOUBLE																					# LiteralDouble
	| LITERAL_FLOAT																						# LiteralFloat
	| LITERAL_LONG																						# LiteralLong
	| LITERAL_INT																						# LiteralInt
	| LITERAL_SHORT																						# LiteralShort
	| LITERAL_BYTE																						# LiteralByte
	| LITERAL_DATE																						# LiteralDate
	| LITERAL_BOOL																						# LiteralBool
	| LITERAL_NULL																						# LiteralNull																
	| type																								# ExpressionType
	
	| scalar_expression NULL_OP scalar_expression														# IfNullOp
	| scalar_expression IF_OP scalar_expression (COLON scalar_expression)?								# IfOp
	| LPAREN type RPAREN scalar_expression																# Cast
	| var_name LPAREN ( scalar_expression ( COMMA scalar_expression )* )? RPAREN						# Function

	| LPAREN scalar_expression RPAREN																	# Parens
	;
	
// Types //
type : (T_BLOB | T_BOOL | T_DATE | T_FLOAT | T_DOUBLE | T_BYTE | T_SHORT | T_INT | T_LONG | T_TEXT | T_STRING) (DOT LITERAL_INT)?;

// ----------------------------------------------------------------------------------------------------- //
// ------------------------------------------ Hyper Expression ----------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
hyper_expression
	: var_name LPAREN (hyper_expression_parameter (COMMA hyper_expression_parameter)*)? RPAREN
	;

hyper_expression_parameter : scalar_expression | matrix_expression | record_expression | table_expression;

// ----------------------------------------------------------------------------------------------------- //
// ------------------------------------------------ NAMES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
table_name : var_name LCURL RCURL;
matrix_name : var_name LBRAC RBRAC;
record_name : var_name LT GT;
var_name : (lib_name DOT)? IDENTIFIER;
lib_name : IDENTIFIER | K_TABLE | T_BLOB | T_BOOL | T_DATE | T_DOUBLE T_BYTE | T_SHORT | T_INT | T_LONG | T_TEXT | T_STRING;
