// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: White Space and Line Terminator inside "grouping" operator are allowed
es5id: 11.1.6_A1
description: >
    Inserting WhiteSpaces and LineTerminators into grouping operator.
    Eval is used
---*/

//CHECK#1
console.log(!(eval("(\u00091\u0009)") !== 1));

//CHECK#2
console.log(!(eval("(\u000B1\u000B)") !== 1));

//CHECK#3
console.log(!(eval("(\u000C1\u000C)") !== 1));

//CHECK#4
console.log(!(eval("(\u00201\u0020)") !== 1));

//CHECK#5
console.log(!(eval("(\u00A01\u00A0)") !== 1));

//CHECK#6
console.log(!(eval("(\u000A1\u000A)") !== 1));

//CHECK#7
console.log(!(eval("(\u000D1\u000D)") !== 1));

//CHECK#8
console.log(!(eval("(\u20281\u2028)") !== 1));

//CHECK#9
console.log(!(eval("(\u20291\u2029)") !== 1));

//CHECK#10
console.log(!(eval("(\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u20291\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029)") !== 1));

