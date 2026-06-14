// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between "-" and UnaryExpression are
    allowed
es5id: 11.4.7_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!(eval("-\u00091") !== -1));

//CHECK#2
console.log(!(eval("-\u000B1") !== -1));

//CHECK#3
console.log(!(eval("-\u000C1") !== -1));

//CHECK#4
console.log(!(eval("-\u00201") !== -1));

//CHECK#5
console.log(!(eval("-\u00A01") !== -1));

//CHECK#6
console.log(!(eval("-\u000A1") !== -1));

//CHECK#7
console.log(!(eval("-\u000D1") !== -1));

//CHECK#8
console.log(!(eval("-\u20281") !== -1));

//CHECK#9
console.log(!(eval("-\u20291") !== -1));

//CHECK#10
console.log(!(eval("-\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u20291") !== -1));

