// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator uses GetValue
es5id: 11.14_A2.1_T3
description: If GetBase(AssigmentExpression) is null, throw ReferenceError
---*/

//CHECK#1
let __result1 = false;
try {
  1, y;
} catch (e) {
  __result1 = !((e instanceof ReferenceError) !== true);
}
console.log(__result1);

