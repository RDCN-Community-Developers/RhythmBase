grammar Expression;

@header {
namespace RhythmBase.RhythmDoctor.Components.Lang;
}

// Entry point: a single expression, used by numeric fields and `{...}` text
// interpolation (the game's FloatExpression) and by CallCustomMethod arguments.
// Side-effecting statements live in the CallCustomMethod grammar.
entry
    : expression? EOF
    ;

// Operator precedence, highest first (the order of left-recursive alternatives).
expression
    : expression ('*' | '/' | '%') expression                                   # mul
    | expression ('+' | '-') expression                                          # add
    | expression ('==' | '!=' | '>' | '<' | '>=' | '<=') expression              # compare
    | expression AND expression                                                  # and
    | expression OR expression                                                   # or
    | NOT expression                                                             # not
    | ('+' | '-') expression                                                     # sign
    | '(' expression ')'                                                         # paren
    | function_call                                                              # call
    | identifier                                                                 # var
    | INT                                                                        # int
    | FLOAT                                                                      # float
    | BOOLEAN                                                                    # bool
    | RANK_ENUM                                                                  # rank
    | QUOTED_STRING                                                              # string
    | PREFIX_STRING                                                              # prefixed
    ;

function_call
    : identifier '(' (expression (',' expression)*)? ')'
    ;

identifier
    : (IDENTIFIER | INT_VARIABLE | FLOAT_VARIABLE | BOOLEAN_VARIABLE) ('.' IDENTIFIER | '[' INT ']')*
    ;

// Logical operators accept both the upper-case RDCode spelling and the mixed-case
// spelling used by the game data.
AND : 'AND' | 'And' ;
OR  : 'OR'  | 'Or'  ;
NOT : 'NOT' | 'Not' ;

// The 30 fixed slots: i0-i9, f0-f9, b0-b9.
INT_VARIABLE     : 'i' [0-9] ;
FLOAT_VARIABLE   : 'f' [0-9] ;
BOOLEAN_VARIABLE : 'b' [0-9] ;

BOOLEAN : 'true' | 'false' | 'True' | 'False' ;

// Rank literals accepted by atLeastRank(), e.g. S+.
RANK_ENUM : 'S+' | 'S' | 'A' | 'B' | 'C' | 'D' | 'F' ;

QUOTED_STRING : '"' ( '\\' . | ~["\\] )* '"' ;
PREFIX_STRING : 'str:' ~[ ",\\)\r\n]* ;

IDENTIFIER : [a-zA-Z_] [a-zA-Z0-9_]* ;

INT   : [0-9]+ ;
FLOAT : [0-9]+ '.' [0-9]+ ;

WS : [ \t\r\n]+ -> skip ;
