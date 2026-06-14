// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: First expression is evaluated first, and then second expression
es5id: 11.10.2_A2.4_T1
description: Checking with "="
---*/

//CHECK#1
var x = 1; 
console.log(!(((x = 0) ^ x) !== 0));

//CHECK#2
var x = 0; 
console.log(!((x ^ (x = 1)) !== 1));
