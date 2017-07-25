lexer grammar PulseLexer;

// -- Reductions -- //
SET_REDUCTIONS 
	: A V G 
	| C O R R 
	| C O U N T 
	| C O U N T '_' A L L 
	| C O U N T '_' N U L L 
	| C O V A R 
	| F I R S T
	| F R E Q 
	| I N T E R C E P T
	| L A S T 
	| M A X 
	| M I N 
	| S L O P E 
	| S T D E V 
	| S U M 
	| V A R
	;

// Opperators //
PLUS : '+';
MINUS : '-';
MUL : '*';
DIV : '/';
DIV2 : '/?';
MOD : '%';
POW : '^';
EQ : '==';
NEQ : '!=';
LT : '<';
LTE : '<=';
GT : '>';
GTE : '>=';
NULL_OP : '??';
IF_OP : '?';
ELSE_OP : ':';
LPAREN : '(';
RPAREN : ')';
LBRAC : '[';
RBRAC : ']';
LCURL : '{';
RCURL : '}';
COMMA : ',';
SEMI_COLON : ';';
CAST : '->';
LAMBDA : '=>';
DOT : '.';
ASSIGN : '=';
TILDA : '~';
PIPE : '|';
OR : O R;
AND : A N D;
XOR : X O R;
NOT : N O T | '!';

// Keywods //
K_APPEND : A P P E N D;
K_AS : A S;
K_ASC : A S C;
K_BY : B Y;
K_CLUSTER : C L U S T E R;
K_CONNECT : C O N N E C T;
K_DESC : D E S C;
K_DISTINCT : D I S T I N C T;
K_DO : D O;
K_ELSE : E L S E;
K_ESCAPE : E X I T;
K_EXEC : E X E C | E X E C U T E;
K_FOLD : F O L D;
K_FOR : F O R;
K_FROM : F R O M;
K_GROUP : G R O U P;
K_IDENTITY : I D E N T I T Y | I D E N T;
K_IF : I F;
K_JOIN : J O I N;
K_ALEFT_JOIN : A L E F T '_' J O I N;
K_LEFT_JOIN : L E F T '_' J O I N;
K_ON : O N;
K_OPEN_TABLE : O P E N '_' T A B L E;
K_ORDER : O R D E R | O R D E R '_' B Y;
K_PRINT : P R I N T;
K_QUERY : Q U E R Y;
K_SELECT : S E L E C T;
K_SIZE : S I Z E;
K_TABLE : T A B L E;
K_THEN : T H E N;
K_TO : T O;
K_UNION : U N I O N;
K_WHERE : W H E R E;
K_WHILE : W H I L E;

// Core types //
T_BLOB : B L O B;
T_BOOL : B O O L;
T_DATE : D A T E;
T_DOUBLE : D O U B L E | N U M;
T_INT : I N T;
T_STRING : S T R I N G;

// Cell Literal Support //
LITERAL_NULL // NULL INT
	: N U L L
	; 
LITERAL_BOOL 
	: T R U E 
	| F A L S E
	;
LITERAL_BLOB 
	: '0' X (HEX HEX)*;
LITERAL_DATE 
	: '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ '\'' T 												// 'YYYY-MM-DD'T
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '\'' T				// 'YYYY-MM-DD:HH:MM:SS'T
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '.' DIGIT+ '\'' T	// 'YYYY-MM-DD:HH:MM:SS.LLLLLLLL'T
	;
LITERAL_DOUBLE 
	: DIGIT+ '.' DIGIT+ (D)?  // DOUBLE
	| (DIGIT+) D			// 'D' MEANS THIS HAS THE FORM OF AN INT, BUT WE WANT IT TO BE A DOUBLE; AVOIDS HAVING TO DO A CAST
	;
LITERAL_INT 
	: DIGIT+ 
	;
LITERAL_STRING 
	: '\'' ( ~'\'' | '\'\'' )* '\'' // NORMAL STRING -> 'abcdef'
	| '"' ( ~'"' | '""')* '"'		// NORMAL STRING -> "ABCDEF"
	| '\'\''						// EMPTY STRING -> ''
	| SLIT .*? SLIT					// COMPLEX STRING LITERAL $$ ANYTHING $$
	| C R L F						// \n
	| T A B							// \t
	;

// Base Token //
PARAMETER 
	: '@' IDENTIFIER
	;
IDENTIFIER
	: [a-zA-Z_] [a-zA-Z_0-9]*
	;

// Comments and whitespace //
SINGLE_LINE_COMMENT : '//' ~[\r\n]* -> channel(HIDDEN);
MULTILINE_COMMENT : '/*' .*? ( '*/' | EOF ) -> channel(HIDDEN);
WS : ( ' ' | '\t' |'\r' | '\n' | '\r\n')* -> channel(HIDDEN);

fragment SLIT : '$$';
fragment DIGIT : [0-9];
fragment HEX : [aAbBcCdDeEfF0123456789];
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];
