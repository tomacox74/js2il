// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator +x returns ToNumber(x)
es5id: 11.4.6_A3_T2
description: Type(x) is number primitive or Number object
---*/

//CHECK#1
console.log(!(+0.1 !== 0.1));

//CHECK#2
console.log(!(+new Number(-1.1) !== -1.1));

