// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When String is called as a function rather than as a constructor, it
    performs a type conversion
es5id: 15.5.1.1_A1_T11
description: Call String(1/0) and String(-1/0), and call with +/-Infinity
---*/

var value = String(1 / 0);
console.log(typeof value === "string");
console.log(value === "Infinity");

value = String(-1 / 0);
console.log(typeof value === "string");
console.log(value === "-Infinity");

value = String(Infinity);
console.log(typeof value === "string");
console.log(value === "Infinity");

value = String(-Infinity);
console.log(typeof value === "string");
console.log(value === "-Infinity");

value = String(Number.POSITIVE_INFINITY);
console.log(typeof value === "string");
console.log(value === "Infinity");

value = String(Number.NEGATIVE_INFINITY);
console.log(typeof value === "string");
console.log(value === "-Infinity");
