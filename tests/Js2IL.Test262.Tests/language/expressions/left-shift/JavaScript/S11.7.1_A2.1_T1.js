// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator x << y uses GetValue
es5id: 11.7.1_A2.1_T1
description: Either Type is not Reference or GetBase is not null
---*/

//CHECK#1
console.log(!(2 << 1 !== 4));

//CHECK#2
var x = 2;
console.log(!(x << 1 !== 4));

//CHECK#3
var y = 1;
console.log(!(2 << y !== 4));

//CHECK#4
var x = 2;
var y = 1;
console.log(!(x << y !== 4));

//CHECK#5
var objectx = new Object();
var objecty = new Object();
objectx.prop = 2;
objecty.prop = 1;
console.log(!(objectx.prop << objecty.prop !== 4));
