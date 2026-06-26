// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When String is called as a function rather than as a constructor, it
    performs a type conversion
es5id: 15.5.1.1_A1_T8
description: Call String(new Array)
---*/

var __old__Array__prototype__toString = Array.prototype.toString;

Array.prototype.toString = function() {
  return "__ARRAY__";
};

var __str = String(new Array);

// restore old toString method just in case
Array.prototype.toString = __old__Array__prototype__toString;

console.log(typeof __str === "string");
console.log(__str === "__ARRAY__");
