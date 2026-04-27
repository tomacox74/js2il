// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Non-strict equality comparison of BigInt and non-finite Number values
esid: sec-abstract-equality-comparison
info: |
  12. If Type(x) is BigInt and Type(y) is Number, or if Type(x) is Number and Type(y) is BigInt,
    a. If x or y are any of NaN, +∞, or -∞, return false.

features: [BigInt]
---*/
console.log(Object.is(0n == Infinity, false));
console.log(Object.is(Infinity == 0n, false));
console.log(Object.is(1n == Infinity, false));
console.log(Object.is(Infinity == 1n, false));
console.log(Object.is(-1n == Infinity, false));
console.log(Object.is(Infinity == -1n, false));
console.log(Object.is(0n == -Infinity, false));
console.log(Object.is(-Infinity == 0n, false));
console.log(Object.is(1n == -Infinity, false));
console.log(Object.is(-Infinity == 1n, false));
console.log(Object.is(-1n == -Infinity, false));
console.log(Object.is(-Infinity == -1n, false));
console.log(Object.is(0n == NaN, false));
console.log(Object.is(NaN == 0n, false));
console.log(Object.is(1n == NaN, false));
console.log(Object.is(NaN == 1n, false));
console.log(Object.is(-1n == NaN, false));
console.log(Object.is(NaN == -1n, false));
