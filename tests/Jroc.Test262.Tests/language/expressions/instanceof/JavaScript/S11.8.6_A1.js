// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between RelationalExpression and
    "instanceof" and between "instanceof" and ShiftExpression are allowed
es5id: 11.8.6_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!(eval("({})\u0009instanceof\u0009Object") !== true));

//CHECK#2
console.log(!(eval("({})\u000Binstanceof\u000BObject") !== true));

//CHECK#3
console.log(!(eval("({})\u000Cinstanceof\u000CObject") !== true));

//CHECK#4
console.log(!(eval("({})\u0020instanceof\u0020Object") !== true));

//CHECK#5
console.log(!(eval("({})\u00A0instanceof\u00A0Object") !== true));

//CHECK#6
console.log(!(eval("({})\u000Ainstanceof\u000AObject") !== true));

//CHECK#7
console.log(!(eval("({})\u000Dinstanceof\u000DObject") !== true));

//CHECK#8
console.log(!(eval("({})\u2028instanceof\u2028Object") !== true));

//CHECK#9
console.log(!(eval("({})\u2029instanceof\u2029Object") !== true));

//CHECK#10
console.log(!(eval("({})\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029instanceof\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029Object") !== true));

