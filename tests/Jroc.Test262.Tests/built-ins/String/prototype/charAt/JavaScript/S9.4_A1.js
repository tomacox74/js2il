// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If ToNumber(value) is NaN, ToInteger(value) returns +0
es5id: 9.4_A1
description: >
    Check what position is defined by Number.NaN in string "abc":
    "abc".charAt(Number.NaN)
---*/

console.log("abc".charAt(Number.NaN) === "a");
console.log("abc".charAt("x") === "a");
