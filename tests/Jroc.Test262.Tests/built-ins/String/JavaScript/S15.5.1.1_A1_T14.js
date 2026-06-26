// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When String is called as a function rather than as a constructor, it
    performs a type conversion
es5id: 15.5.1.1_A1_T14
description: Call String(0) and String(-0)
---*/

var __str = String(0);
console.log(typeof __str === "string");
console.log(__str === "0");

__str = String(-0);
console.log(typeof __str === "string");
console.log(__str === "0");
