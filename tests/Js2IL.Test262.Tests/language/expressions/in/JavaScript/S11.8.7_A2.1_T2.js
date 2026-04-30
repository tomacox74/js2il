// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator "in" uses GetValue
es5id: 11.8.7_A2.1_T2
description: If GetBase(RelationalExpression) is null, throw ReferenceError
---*/

//CHECK#1
let __result1 = false;
try {
  MAX_VALUE in Number;
} catch (e) {
  __result1 = !((e instanceof ReferenceError) !== true);
}
console.log(__result1);

