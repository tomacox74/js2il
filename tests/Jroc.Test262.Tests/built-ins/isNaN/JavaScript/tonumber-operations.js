// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-isnan-number
description: >
  number argument is converted by ToNumber
info: |
  isNaN (number)

  1. Let num be ? ToNumber(number).
  2. If num is NaN, return true.
  3. Otherwise, return false.
---*/

console.log(Object.is(isNaN("0"), false));
console.log(Object.is(isNaN(""), false));
console.log(Object.is(isNaN("Infinity"), false));
console.log(Object.is(isNaN("this is not a number"), true));
console.log(Object.is(isNaN(true), false));
console.log(Object.is(isNaN(false), false));
console.log(Object.is(isNaN([1]), false));
console.log(Object.is(isNaN([Infinity]), false));
console.log(Object.is(isNaN([NaN]), true));
console.log(Object.is(isNaN(null), false));
console.log(Object.is(isNaN(undefined), true));
console.log(Object.is(isNaN(), true));
