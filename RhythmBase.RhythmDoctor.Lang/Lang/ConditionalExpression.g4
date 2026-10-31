grammar ConditionalExpression;
import Expression;

// A complete Conditional_Custom string. Apart from the two player-mode keywords,
// every accepted form is an ordinary expression that is interpreted as a boolean:
//   atLeastNPerfects(n, count), atLeastRank(S+), passedLevel(Name), buttonPressCount>2,
//   neverRun, narration, booth, paigeLeaves, voiceLanguage==English, platform==Windows, ...
conditional
    : mode_keyword EOF
    | expression EOF
    ;

mode_keyword
    : TWO_P_MODE
    | ONE_P_MODE
    ;

TWO_P_MODE : '2pMode' ;
ONE_P_MODE : '1pMode' ;
