// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator uses GetValue
es5id: 11.14_A2.1_T1
description: Either Expression is not Reference or GetBase is not null
---*/

console.log(Object.is((1, 2), 2));

var x = 1;
console.log(Object.is((x, 2), 2));

var y = 2;
console.log(Object.is((1, y), 2));

var x = 1;
var y = 2;
console.log(Object.is((x, y), 2));

var x = 1;
console.log(Object.is((x, x), 1));

var objectx = new Object();
var objecty = new Object();
objectx.prop = true;
objecty.prop = 1.1;
console.log(Object.is((objectx.prop = false, objecty.prop), objecty.prop));
console.log(Object.is(objectx.prop, false));
