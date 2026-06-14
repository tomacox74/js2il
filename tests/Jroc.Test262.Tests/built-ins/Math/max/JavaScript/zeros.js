// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-math.max
description: >
  +0 is considered to be larger than -0
info: |
  Math.max ( value1, value2 , …values )

  The comparison of values to determine the largest value is done using the
  Abstract Relational Comparison algorithm except that +0 is considered to be
  larger than -0.
---*/

console.log(Object.is(Math.max(0, 0), 0));
console.log(Object.is(Math.max(-0, -0), -0));
console.log(Object.is(Math.max(0, -0), 0));
console.log(Object.is(Math.max(-0, 0), 0));
console.log(Object.is(Math.max(0, 0, -0), 0));
