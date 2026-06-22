// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Number([value]) returns a number value (not a Number object) computed by
    ToNumber(value) if value was supplied
es5id: 15.7.1.1_A1
description: Used values "10", 10, new String("10"), new Object(10) and "abc"
---*/

console.log(typeof Number("10") === "number");
console.log(typeof Number(10) === "number");
console.log(typeof Number(new String("10")) === "number");
console.log(typeof Number(new Object(10)) === "number");
console.log(Object.is(Number("abc"), NaN));
console.log(Object.is(Number("INFINITY"), NaN));
console.log(Object.is(Number("infinity"), NaN));
