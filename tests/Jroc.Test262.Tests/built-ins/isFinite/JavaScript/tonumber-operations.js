// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-isfinite-number
description: >
  number argument is converted by ToNumber
info: |
  isFinite (number)

  1. Let num be ? ToNumber(number).
  2. If num is NaN, +∞, or -∞, return false.
  3. Otherwise, return true.
---*/

console.log(Object.is(isFinite("0"), true));
console.log(Object.is(isFinite(""), true));
console.log(Object.is(isFinite("Infinity"), false));
console.log(Object.is(isFinite("this is not a number"), false));
console.log(Object.is(isFinite(true), true));
console.log(Object.is(isFinite(false), true));
console.log(Object.is(isFinite([1]), true));
console.log(Object.is(isFinite([Infinity]), false));
console.log(Object.is(isFinite([NaN]), false));
console.log(Object.is(isFinite(null), true));
console.log(Object.is(isFinite(undefined), false));
console.log(Object.is(isFinite(), false));
