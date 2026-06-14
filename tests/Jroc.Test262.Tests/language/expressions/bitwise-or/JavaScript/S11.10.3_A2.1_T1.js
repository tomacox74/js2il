// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator x | y uses GetValue
es5id: 11.10.3_A2.1_T1
description: Either Type is not Reference or GetBase is not null
---*/

//CHECK#1
console.log(!((1 | 0) !== 1));

//CHECK#2
var x = 1;
console.log(!((x | 0) !== 1));

//CHECK#3
var y = 0;
console.log(!((1 | y) !== 1));

//CHECK#4
var x = 1;
var y = 0;
console.log(!((x | y) !== 1));

//CHECK#5
var objectx = new Object();
var objecty = new Object();
objectx.prop = 1;
objecty.prop = 0;
console.log(!((objectx.prop | objecty.prop) !== 1));
