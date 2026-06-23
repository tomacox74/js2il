// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When the Object(value) is called and the value is null, undefined or not supplied,
    create and return a new Object object if the object constructor had been called with the same arguments (15.2.2.1)
es5id: 15.2.1.1_A1_T3
description: Creating Object() and checking its properties
---*/

var __obj = Object();
var n__obj = new Object();

console.log(__obj.toString() === n__obj.toString());
console.log(__obj.constructor === n__obj.constructor);
console.log(__obj.prototype === n__obj.prototype);
console.log(__obj.toLocaleString() === n__obj.toLocaleString());
console.log(typeof __obj === typeof n__obj);
