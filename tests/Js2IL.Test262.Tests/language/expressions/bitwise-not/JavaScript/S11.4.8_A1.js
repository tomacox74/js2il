// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between "~" and UnaryExpression are
    allowed
es5id: 11.4.8_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!(eval("~\u00090") !== -1));

//CHECK#2
console.log(!(eval("~\u000B0") !== -1));

//CHECK#3
console.log(!(eval("~\u000C0") !== -1));

//CHECK#4
console.log(!(eval("~\u00200") !== -1));

//CHECK#5
console.log(!(eval("~\u00A00") !== -1));

//CHECK#6
console.log(!(eval("~\u000A0") !== -1));

//CHECK#7
console.log(!(eval("~\u000D0") !== -1));

//CHECK#8
console.log(!(eval("~\u20280") !== -1));

//CHECK#9
console.log(!(eval("~\u20290") !== -1));

//CHECK#10
console.log(!(eval("~\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u20290") !== -1));

