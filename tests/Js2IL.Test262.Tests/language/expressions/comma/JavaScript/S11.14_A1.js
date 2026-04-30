// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    White Space and Line Terminator between Expression and , or between , and
    AssignmentExpression are allowed
es5id: 11.14_A1
description: Checking by using eval
---*/

//CHECK#1
console.log(!((eval("false\u0009,\u0009true")) !== true));

//CHECK#2
console.log(!((eval("false\u000B,\u000Btrue")) !== true));

//CHECK#3
console.log(!((eval("false\u000C,\u000Ctrue")) !== true));

//CHECK#4
console.log(!((eval("false\u0020,\u0020true")) !== true));

//CHECK#5
console.log(!((eval("false\u00A0,\u00A0true")) !== true));

//CHECK#6
console.log(!((eval("false\u000A,\u000Atrue")) !== true));

//CHECK#7
console.log(!((eval("false\u000D,\u000Dtrue")) !== true));

//CHECK#8
console.log(!((eval("false\u2028,\u2028true")) !== true));

//CHECK#9
console.log(!((eval("false\u2029,\u2029true")) !== true));


//CHECK#10
console.log(!((eval("false\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029,\u0009\u000B\u000C\u0020\u00A0\u000A\u000D\u2028\u2029true")) !== true));

