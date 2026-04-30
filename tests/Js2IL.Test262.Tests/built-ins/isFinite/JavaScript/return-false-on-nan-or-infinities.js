// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-isfinite-number
description: >
  Return false if number is NaN, Infinity or -Infinity
info: |
  isFinite (number)

  1. Let num be ? ToNumber(number).
  2. If num is NaN, +∞, or -∞, return false.
---*/

console.log(Object.is(isFinite(NaN), false));
console.log(Object.is(isFinite(Infinity), false));
console.log(Object.is(isFinite(-Infinity), false));
