// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator +x returns ToNumber(x)
es5id: 11.4.6_A3_T3
description: Type(x) is string primitive or String object
---*/

console.log(Object.is(+"1", 1));
console.log(Object.is(+new Number("-1"), -1));
console.log(Object.is(isNaN(+"x"), true));
console.log(Object.is(isNaN(+"INFINITY"), true));
console.log(Object.is(isNaN(+"infinity"), true));
