// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When String is called as a function rather than as a constructor, it
    performs a type conversion
es5id: 15.5.1.1_A1_T13
description: Call String(true) and String(false)
---*/

var value = String(true);
console.log(typeof value === "string");
console.log(value === "true");

value = String(false);
console.log(typeof value === "string");
console.log(value === "false");

value = String(Boolean(true));
console.log(typeof value === "string");
console.log(value === "true");

value = String(Boolean(false));
console.log(typeof value === "string");
console.log(value === "false");
