parser grammar PulseParser;

options
{
	tokenVocab = PulseLexer;
}

compileUnit
	: scalar_expression
	| matrix_expression
	| record_expression
	| table_expression
	| (action_expression SEMI_COLON)+
	| EOF
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- ACTIONS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
action_expression
	: K_NEW scalar_name ASSIGN scalar_expression 																# DeclareScalar
	| K_NEW matrix_name ASSIGN matrix_expression 																# DeclareMatrix
	| K_NEW record_name ASSIGN record_expression																# DeclareRecord
	| K_NEW table_name ASSIGN LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL								# DeclareTable1
	| K_NEW table_name ASSIGN table_expression																	# DeclareTable2
	
	| scalar_name assignment scalar_expression 																	# ActionScalarAssign
	| scalar_name increment 																					# ActionScalarIncrement
	| matrix_name LBRAC scalar_expression COMMA scalar_expression RBRAC assignment scalar_expression 			# ActionMatrixUnit2DAssign
	| matrix_name LBRAC scalar_expression COMMA scalar_expression RBRAC increment 								# ActionMatrixUnit2DIncrement
	| matrix_name LBRAC scalar_expression RBRAC assignment scalar_expression 									# ActionMatrixUnit1DAssign
	| matrix_name LBRAC scalar_expression RBRAC increment 														# ActionMatrixUnit1DIncrement	
	
	| K_PRINT scalar_expression (K_TO scalar_expression)? 														# ActionPrintScalar
	| K_PRINT matrix_expression (K_TO scalar_expression)? 														# ActionPrintMatrix
	| K_PRINT record_expression (K_TO scalar_expression)? 														# ActionPrintRecord
	| K_PRINT table_expression (K_TO scalar_expression)? 														# ActionPrintTable

	| table_name PLUS ASSIGN record_expression																	# ActionTableInsertRecord
	| table_name PLUS ASSIGN table_expression																	# ActionTableInsertTable
	
	//| var_name LPAREN (expr (COMMA expr)*)? RPAREN											# ActionCallSeq
	//| var_name LPAREN (parameter_name (COMMA parameter_name)*)? RPAREN						# ActionCallNamed
	
	/*
		both <set> {} and <do> DO {}; are nested actions but only DO can be used in alone: <set> must be a child in an scalar_expression tree;
	*/
	| LCURL (action_expression SEMI_COLON)+ RCURL																			# ActionSet				
	| K_DO LCURL (action_expression SEMI_COLON)+ RCURL																		# ActionDo
	
	| K_FOR_EACH record_name K_IN table_expression LCURL (action_expression SEMI_COLON)+ RCURL								# ActionForEachRecord
	| K_FOR_EACH scalar_name K_IN matrix_name LCURL (action_expression SEMI_COLON)+ RCURL									# ActionForEachMatrix
	| K_FOR_EACH scalar_name K_IN matrix_expression LCURL (action_expression SEMI_COLON)+ RCURL								# ActionForEachMatrixExpression

	| K_FOR LPAREN (type)? scalar_name ASSIGN scalar_expression SEMI_COLON 
		scalar_expression SEMI_COLON action_expression RPAREN LCURL (action_expression SEMI_COLON)+ RCURL 					# ActionFor		
	| K_WHILE LPAREN scalar_expression RPAREN LCURL (action_expression SEMI_COLON)+ RCURL									# ActionWhile
	| K_IF LPAREN scalar_expression RPAREN action_expression 
		(K_ELSE K_IF LPAREN scalar_expression RPAREN action_expression)* (K_ELSE action_expression)?						# ActionIF
	;

sub_if : K_IF LCURL action_expression+ RCURL;
parameter_name : PARAM lib_name ASSIGN scalar_expression;

// Compound Opperators //
assignment : (ASSIGN | PLUS ASSIGN | MINUS ASSIGN | MUL ASSIGN | DIV ASSIGN | DIV2 ASSIGN | MOD ASSIGN);
increment : (PLUS PLUS | MINUS MINUS);

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- TABLES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
table_expression
	: table_expression COLON IDENTIFIER (MUL | NOT | PIPE) table_expression COLON IDENTIFIER 
		AMPER jframe LCURL nframe RCURL (where)? oframe?														# TableExpressionJoin
	| table_expression DIV LCURL nframe RCURL MOD LCURL aframe RCURL where? oframe?								# TableExpressionFold1
	| table_expression DIV LCURL nframe RCURL where? oframe?													# TableExpressionFold2
	| table_expression MOD LCURL aframe RCURL where? oframe?													# TableExpressionFold3
	| table_expression LCURL nframe RCURL where? oframe?														# TableExpressionSelect1
	| table_expression where oframe?																			# TableExpressionSelect2
	| table_expression oframe																					# TableExpressionSelect3
	| table_expression (PLUS table_expression)+																	# TableExpressionUnion
	| TABLE_TOK LCURL nframe RCURL (PLUS LCURL nframe RCURL)*													# TableExpressionLiteral
	| table_name LPAREN ( param (COMMA param)*)? RPAREN															# TableExpressionFunction
	//| LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL (COLON db_name)?										# TableExpressionCTOR
	| table_name																								# TableExpressionLookup
	| LPAREN table_expression RPAREN																			# TableExpressionParens
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- MATRIX ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
 matrix_expression
	: NOT matrix_expression																	# MatrixInvert
	| TILDA matrix_expression																# MatrixTranspose
	| matrix_expression MUL MUL matrix_expression											# MatrixTrueMul
	| matrix_expression op=(MUL | DIV | DIV2 | MOD) matrix_expression						# MatrixMulDiv
	| matrix_expression op=(PLUS | MINUS) matrix_expression									# MatrixAddSub
	| matrix_name																			# MatrixLookup
	| matrix_name LPAREN ( param (COMMA param)*)? RPAREN									# MatrixExpressionFunction
	| MATRIX_TOK LCURL nframe RCURL (PLUS LCURL nframe RCURL)*								# MatrixLiteral
	| type LBRAC scalar_expression (COMMA scalar_expression)? RBRAC							# MatrixCTOR
	| LPAREN matrix_expression RPAREN														# MatrixParen
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- RECORDS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
record_expression
	: RECORD_TOK LCURL nframe RCURL												# RecordExpressionLiteral			// @ { , , , }
	| record_name																# RecordExpressionLookup			// @r
	| record_expression (MUL record_expression)+								# RecordExpressionUnion				// @r + @s + @t
	| record_name LPAREN ( param (COMMA param)*)? RPAREN						# RecordExpressionFunction
	| LPAREN record_expression RPAREN											# RecordExpressionParens			// (@r)
	;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- FRAMES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //

jframe : jelement (AND jelement)*;
jelement : scalar_name EQ scalar_name;

nframe : nelement (COMMA nelement)*;
nelement : scalar_expression (K_AS | COLON IDENTIFIER)?;

aframe : agg (COMMA agg)*;
agg : SET_REDUCTIONS LPAREN (nframe)? RPAREN (where)? (K_AS | COLON IDENTIFIER)?;

where: (K_WHERE | AMPER) scalar_expression;

oframe : (K_ORDER | PIPE) order (COMMA order)*;
order : LITERAL_INT (K_ASC | K_DESC)?;

// ----------------------------------------------------------------------------------------------------- //
// ----------------------------------------------- SCALARS --------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
            
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
	| scalar_expression (L_SHIFT | R_SHIFT | L_ROTATE | R_ROTATE) scalar_expression						# BitShiftRotate	// X << Y
	
	| scalar_name																						# TableOrScalarMember	// X.Name
	| record_name DOT IDENTIFIER																		# RecordMember			// @R.Name
	| matrix_expression LBRAC scalar_expression (COMMA scalar_expression)? RBRAC						# MatrixMember			// $M[]

	| LITERAL_CSTRING																					# LiteralCString
	| LITERAL_BSTRING																					# LiteralBString
	| LITERAL_BINARY																					# LiteralBinary
	| LITERAL_DOUBLE																					# LiteralDouble
	| LITERAL_SINGLE																					# LiteralSingle
	| LITERAL_LONG																						# LiteralLong
	| LITERAL_INT																						# LiteralInt
	| LITERAL_SHORT																						# LiteralShort
	| LITERAL_BYTE																						# LiteralByte
	| LITERAL_DATE_TIME																					# LiteralDateTime
	| LITERAL_BOOL																						# LiteralBool
	| LITERAL_NULL																						# LiteralNull																
	| type																								# ExpressionType
	
	| scalar_expression NULL_OP scalar_expression														# IfNullOp
	| scalar_expression IF_OP scalar_expression (COLON scalar_expression)?								# IfOp
	| LPAREN type RPAREN scalar_expression																# Cast
	//| scalar_name LPAREN ( scalar_expression ( COMMA scalar_expression )* )? RPAREN						# Function
	| scalar_name LPAREN ( param (COMMA param)*)? RPAREN												# ScalarExpressionFunction

	| LPAREN scalar_expression RPAREN																	# Parens
	;

// ----------------------------------------------------------------------------------------------------- //
// -------------------------------------------- Parameters -------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
param : scalar_expression | matrix_expression | record_expression | table_expression;


// ----------------------------------------------------------------------------------------------------- //
// -------------------------------------------- Expressions -------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
//expr : 

//	// Literal
//	sliteral																		# ExpressionScalarLiteral
//	| LCURL nframe RCURL															# ExpressionRecordLiteral
//	| LCURL nframe RCURL PIPE (LCURL nframe RCURL)+									# ExpressionMatrixLiteral

//	// CTOR //
//	| type LBRAC expr (COMMA expr)? RBRAC											# ExpressionCTORMatrix
//	| LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL							# ExpressionCTORRecord
//	| LCURL type IDENTIFIER (COMMA type IDENTIFIER)* RCURL
//		COLON var_name oframe?														# ExpressionCTORTable

//	// Table only expressions //	
//	| K_OPEN LPAREN IDENTIFIER DOT IDENTIFIER RPAREN								# ExpressionTableOpen
//	| LPAREN expr COLON IDENTIFIER (MUL | NOT | PIPE) expr COLON IDENTIFIER 
//		AMPER jframe  RPAREN LCURL nframe RCURL where? oframe? (COLON var_name)?	# ExpressionJoin				// (X : A ** Y : B & A.KEY == B.KEY)[A.KEY, B.VALUE] && B.VALUE >= 100
//	| LPAREN expr DIV LCURL nframe RCURL MOD LCURL aframe RCURL	RPAREN where? oframe? (COLON var_name)?				# ExpressionGroup1				// (X : A / { B, C, D } % { E, F, G }) & (X == Y) | (0,1,2)
//	| LPAREN expr DIV LCURL nframe RCURL RPAREN where? oframe? (COLON var_name)?	# ExpressionGroup2				// (X : A / { B, C, D }) & (X == Y) | (0,1,2)
//	| LPAREN expr MOD LCURL aframe RCURL RPAREN where? oframe? (COLON var_name)?	# ExpressionGroup3				// (X : A / { E, F, G }) & (X == Y) | (0,1,2)
//	| expr LCURL nframe RCURL where? oframe? (COLON var_name)?						# ExpressionSelect	
	
//	// Record only //
//	| LCURL nframe RCURL															# ExpressionFrame	
	
//	// Scalar and Matrix
//	| op=(NOT | PLUS | MINUS | QUESTION) expr										# ExpressionUniary				// -X
//	| expr POW expr																	# ExpressionPower				// X ^ Y
//	| expr op=(MUL | DIV | MOD | DIV2) expr											# ExpressionMultDivMod			// X / Y
//	| expr op=(PLUS | MINUS) expr													# ExpressionAddSub				// X + Y
//	| expr op=(GT | GTE | LT | LTE) expr											# ExpressionGreaterLesser		// X < Y
//	| expr op=(EQ | NEQ) expr														# ExpressionEquality			// X == Y
//	| expr AND expr																	# ExpressionLogicalAnd			// X && Y
//	| expr op=(OR | XOR) expr														# ExpressionLogicalOr			// X || Y

//	// Member Acess //
//	| expr LBRAC expr (COMMA expr)? RBRAC											# ExpressionMatrixMember
//	| IDENTIFIER																	# ExpressionName1				// X
//	| lib_name DOT IDENTIFIER														# ExpressionName2				// X.Y
//	| lib_name DOT IDENTIFIER DOT IDENTIFIER										# ExpressionName3				// X.Y.Z
	
//	// Functional //
//	| var_name LPAREN (expr (COMMA expr)*)? RPAREN									# ExpressionFunction
//	| expr QUESTION expr COLON expr													# ExpressionIf
//	| LPAREN type RPAREN expr														# ExpressionCast

//	// Other //
//	| LPAREN expr RPAREN															# ExpressionParens
//	;

//jframe : jelement (AND jelement)*;
//jelement : var_name EQ var_name;

//nframe : nelement (COMMA nelement)*;
//nelement : expr (K_AS | COLON IDENTIFIER)?;

//aframe : agg (COMMA agg)*;
//agg : SET_REDUCTIONS LPAREN (nframe)? RPAREN (where)? (K_AS | COLON IDENTIFIER)?;

//where: (K_WHERE | AMPER) expr;

//oframe : (K_ORDER | PIPE) order (COMMA order)*;
//order : LITERAL_INT (K_ASC | K_DESC)?;


// Types //
sliteral : (LITERAL_CSTRING | LITERAL_BSTRING | LITERAL_BINARY | LITERAL_DOUBLE | LITERAL_SINGLE | LITERAL_LONG | LITERAL_INT | LITERAL_SHORT | LITERAL_BYTE | LITERAL_DATE_TIME | LITERAL_BOOL | LITERAL_NULL);
hyper_type : (K_TABLE | K_MATRIX | K_RECORD | K_SCALAR);
type : (T_BINARY | T_BOOL | T_DATE | T_SINGLE | T_DOUBLE | T_BYTE | T_SHORT | T_INT | T_LONG | T_BSTRING | T_CSTRING) (DOT LITERAL_INT)?;

// ----------------------------------------------------------------------------------------------------- //
// ------------------------------------------------ NAMES ---------------------------------------------- //
// ----------------------------------------------------------------------------------------------------- //
table_name : (lib_name DOT)? TABLE_TOK IDENTIFIER;
record_name : (lib_name DOT)? RECORD_TOK IDENTIFIER;
matrix_name : (lib_name DOT)? MATRIX_TOK IDENTIFIER;
scalar_name : (lib_name DOT)? IDENTIFIER;
db_name : IDENTIFIER DOT IDENTIFIER;
lib_name : IDENTIFIER | K_TABLE | T_BLOB | T_BOOL | T_DATE | T_DOUBLE T_BYTE | T_SHORT | T_INT | T_LONG | T_TEXT | T_STRING;
