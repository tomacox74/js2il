// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between "!" and UnaryExpression are
    allowed
es5id: 11.4.9_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!(eval("!\u0009true") !== false));

//CHECK#2
console.log(!(eval("!\u000Btrue") !== false));

//CHECK#3
console.log(!(eval("!\u000Ctrue") !== false));

//CHECK#4
console.log(!(eval("!\u0020true") !== false));

//CHECK#5
console.log(!(eval("!\u00A0true") !== false));

//CHECK#6
console.log(!(eval("!\u000Atrue") !== false));

//CHECK#7
console.log(!(eval("!\u000Dtrue") !== false));

//CHECK#8
console.log(!(eval("!\u2028true") !== false));

//CHECK#9
console.log(!(eval("!\u2029true") !== false));

//CHECK#10
console.log(!(eval("!\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029true") !== false));

