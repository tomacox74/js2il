// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When String is called as a function rather than as a constructor, it
    performs a type conversion
es5id: 15.5.1.1_A1_T12
description: Call String(1/"a"), String("b"* null) and String(Number.NaN)
---*/

var value = String(1 / "a");
console.log(typeof value === "string");
console.log(value === "NaN");

value = String("b" * null);
console.log(typeof value === "string");
console.log(value === "NaN");

value = String(Number.NaN);
console.log(typeof value === "string");
console.log(value === "NaN");
