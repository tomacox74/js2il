// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Operator !x uses GetValue
es5id: 11.4.9_A2.1_T1
description: Either Type(x) is not Reference or GetBase(x) is not null
---*/

//CHECK#1
console.log(!(!true !== false));

//CHECK#2
console.log(!(!(!true) !== true));

//CHECK#3
var x = true;
console.log(!(!x !== false));

//CHECK#4
var x = true;
console.log(!(!(!x) !== true));

//CHECK#5
var object = new Object();
object.prop = true;
console.log(!(!object.prop !== false));

