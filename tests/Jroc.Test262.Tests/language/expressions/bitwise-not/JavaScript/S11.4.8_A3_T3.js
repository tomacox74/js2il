// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator ~x returns ~ToInt32(x)
es5id: 11.4.8_A3_T3
description: Type(x) is string primitive or String object
---*/

console.log(Object.is(~"1", -2));
console.log(Object.is(~new String("0"), -1));
console.log(Object.is(~"x", -1));
console.log(Object.is(~"", -1));
console.log(Object.is(~new String("-2"), 1));
