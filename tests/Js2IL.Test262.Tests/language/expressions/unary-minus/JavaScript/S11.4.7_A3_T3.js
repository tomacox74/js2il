// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator -x returns -ToNumber(x)
es5id: 11.4.7_A3_T3
description: Type(x) is string primitive or String object
---*/

console.log(Object.is(-"1", -1));
console.log(Object.is(isNaN(-"x"), true));
console.log(Object.is(-new String("-1"), 1));
