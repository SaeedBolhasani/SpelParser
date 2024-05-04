grammar SpelGrammer;

query
: expression
;

expression
: comparision #Comparative
| expression AND expression #And
| expression OR expression  #Or
| '(' expression ')' #Parenthesis
;

comparision
: Field=field Operator='>'	Constant=constant #GreaterThanExpression
| Field=field Operator='>='	Constant=constant #GreaterThanOrEqualExpression
| Field=field Operator='<'	Constant=constant #LessThanExpression
| Field=field Operator='<='	Constant=constant #LessThanOrEqualExpression
| Field=field Operator='=='	Constant=constant #EqualExpression
| Field=field Operator='!='	Constant=constant #NotEqualExpression
;

field : FIELD;

constant
: STRING #String
| NUMBER #Number
;

OR : O R;
AND : A N D;

WS  : (' '|'\t'|'\r'|'\n')+ -> skip;

STRING : '"' .*? '"' |  '\'' .*? '\'';

FIELD : ([a-zA-Z]|'_')([a-zA-Z]|'_'|[0-9])*;

SIGN
: ('+' | '-')
;

NUMBER
: SIGN? ( [0-9]* '.' )? [0-9]+;

fragment A          : ('A'|'a') ;
fragment N          : ('N'|'n') ;
fragment D          : ('D'|'d') ;

fragment O          : ('O'|'o') ;
fragment R          : ('R'|'r') ;