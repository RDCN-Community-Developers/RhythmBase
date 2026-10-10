grammar CallCustomMethod;

import Expression;

// A single CallCustomMethod operation. The game stores `methodName` as exactly one of:
//   target = expression        (field assignment)
//   target++ / target--        (increment / decrement)
//   function(args)             (void function call)
// `target` is a fixed slot (i0..b9) or a dotted/indexed name
// (level.foo, room[0].foo, vfx.foo) resolved by the execution environment.
program
    : statement EOF
    ;

statement
    : assignment
    | increment
    | decrement
    | expression
    ;

assignment : identifier '=' expression ;
increment  : identifier '++' ;
decrement  : identifier '--' ;
