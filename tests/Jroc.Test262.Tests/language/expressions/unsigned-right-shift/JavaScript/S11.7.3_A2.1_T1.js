// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator x >>> y uses GetValue
es5id: 11.7.3_A2.1_T1
description: Either Type is not Reference or GetBase is not null
---*/

//CHECK#1
console.log(!(-4 >>> 1 !== 2147483646));

//CHECK#2
var x = -4;
console.log(!(x >>> 1 !== 2147483646));

//CHECK#3
var y = 1;
console.log(!(-4 >>> y !== 2147483646));

//CHECK#4
var x = -4;
var y = 1;
console.log(!(x >>> y !== 2147483646));

//CHECK#5
var objectx = new Object();
var objecty = new Object();
objectx.prop = -4;
objecty.prop = 1;
console.log(!(objectx.prop >>> objecty.prop !== 2147483646));
