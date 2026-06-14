// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator ~x returns ~ToInt32(x)
es5id: 11.4.8_A3_T2
description: Type(x) is number primitive or Number object
---*/

//CHECK#1
console.log(!(~0.1 !== -1));

//CHECK#2
console.log(!(~new Number(-0.1) !== -1));

//CHECK#3
console.log(!(~NaN !== -1));

//CHECK#4
console.log(!(~new Number(NaN) !== -1));

//CHECK#5
console.log(!(~1 !== -2));

//CHECK#6
console.log(!(~new Number(-2) !== 1));

//CHECK#7
console.log(!(~Infinity !== -1));

