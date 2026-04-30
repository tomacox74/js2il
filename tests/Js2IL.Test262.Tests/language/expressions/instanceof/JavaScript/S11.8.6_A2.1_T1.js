// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator "instanceof" uses GetValue
es5id: 11.8.6_A2.1_T1
description: Either Expression is not Reference or GetBase is not null
---*/

//CHECK#1
console.log(!(({}) instanceof Object !== true));

//CHECK#2
var object = {};
console.log(!(object instanceof Object !== true));

//CHECK#3
var OBJECT = Object;
console.log(!(({}) instanceof OBJECT !== true));

//CHECK#4
var object = {};
var OBJECT = Object;
console.log(!(object instanceof OBJECT !== true));

