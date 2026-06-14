// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between RelationalExpression and "in" and
    between "in" and ShiftExpression are allowed
es5id: 11.8.7_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!(eval("'MAX_VALUE'\u0009in\u0009Number") !== true));

//CHECK#2
console.log(!(eval("'MAX_VALUE'\u000Bin\u000BNumber") !== true));

//CHECK#3
console.log(!(eval("'MAX_VALUE'\u000Cin\u000CNumber") !== true));

//CHECK#4
console.log(!(eval("'MAX_VALUE'\u0020in\u0020Number") !== true));

//CHECK#5
console.log(!(eval("'MAX_VALUE'\u00A0in\u00A0Number") !== true));

//CHECK#6
console.log(!(eval("'MAX_VALUE'\u000Ain\u000ANumber") !== true));

//CHECK#7
console.log(!(eval("'MAX_VALUE'\u000Din\u000DNumber") !== true));

//CHECK#8
console.log(!(eval("'MAX_VALUE'\u2028in\u2028Number") !== true));

//CHECK#9
console.log(!(eval("'MAX_VALUE'\u2029in\u2029Number") !== true));

//CHECK#10
console.log(!(eval("'MAX_VALUE'\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029in\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029Number") !== true));

