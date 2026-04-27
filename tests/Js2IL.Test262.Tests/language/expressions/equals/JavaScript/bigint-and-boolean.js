// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Non-strict equality comparison of BigInt and Boolean values
esid: sec-abstract-equality-comparison
info: |
  8. If Type(x) is Boolean, return the result of the comparison ToNumber(x) == y.
  9. If Type(y) is Boolean, return the result of the comparison x == ToNumber(y).
  ...
  12. If Type(x) is BigInt and Type(y) is Number, or if Type(x) is Number and Type(y) is BigInt,
    ...
    b. If the mathematical value of x is equal to the mathematical value of y, return true, otherwise return false.

features: [BigInt]
---*/
console.log(Object.is(-1n == false, false));
console.log(Object.is(false == -1n, false));
console.log(Object.is(-1n == true, false));
console.log(Object.is(true == -1n, false));
console.log(Object.is(0n == false, true));
console.log(Object.is(false == 0n, true));
console.log(Object.is(0n == true, false));
console.log(Object.is(true == 0n, false));
console.log(Object.is(1n == false, false));
console.log(Object.is(false == 1n, false));
console.log(Object.is(1n == true, true));
console.log(Object.is(true == 1n, true));
console.log(Object.is(2n == false, false));
console.log(Object.is(false == 2n, false));
console.log(Object.is(2n == true, false));
console.log(Object.is(true == 2n, false));
