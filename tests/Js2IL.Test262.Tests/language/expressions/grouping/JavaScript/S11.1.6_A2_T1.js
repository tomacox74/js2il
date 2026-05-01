// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "This" operator doesn't use GetValue. The operators "delete" and "typeof"
    can be applied to parenthesised expressions
es5id: 11.1.6_A2_T1
description: >
    Applying "delete" and "typeof" operators to an undefined variable
    and a property of an object
---*/

//CHECK#1
console.log(!(typeof (x) !== "undefined"));

var object = {};
//CHECK#2
console.log(!(delete (object.prop) !== true));

//CHECK#3
console.log(!(typeof (object.prop) !== "undefined"));

