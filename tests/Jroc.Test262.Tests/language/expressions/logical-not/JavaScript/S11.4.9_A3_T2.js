// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator !x returns !ToBoolean(x)
es5id: 11.4.9_A3_T2
description: Type(x) is number primitive or Number object
---*/

//CHECK#1
console.log(!(!0.1 !== false));

//CHECK#2
console.log(!(!new Number(-0.1) !== false));

//CHECK#3
console.log(!(!NaN !== true));

//CHECK#4
console.log(!(!new Number(NaN) !== false));

//CHECK#5
console.log(!(!0 !== true));

//CHECK#6
console.log(!(!new Number(0) !== false));

//CHECK#7
console.log(!(!Infinity !== false));

