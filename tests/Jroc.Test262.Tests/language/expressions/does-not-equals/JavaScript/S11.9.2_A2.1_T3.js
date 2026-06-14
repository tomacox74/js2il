// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator x != y uses GetValue
es5id: 11.9.2_A2.1_T3
description: If GetBase(y) is null, throw ReferenceError
---*/

//CHECK#1
try {
  1 != y;
  throw new Error("Test262 failure");  
}
catch (e) {
  console.log(!((e instanceof ReferenceError) !== true));
}
