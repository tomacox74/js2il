// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-number.isinteger
description: >
  Return false if argument is not Number
info: |
  Number.isInteger ( number )

  1. If Type(number) is not Number, return false.
  [...]
features: [Symbol]
---*/

console.log(Object.is(Number.isInteger("1"), false));
console.log(Object.is(Number.isInteger([1]), false));
console.log(Object.is(Number.isInteger(new Number(42)), false));
console.log(Object.is(Number.isInteger(false), false));
console.log(Object.is(Number.isInteger(true), false));
console.log(Object.is(Number.isInteger(undefined), false));
console.log(Object.is(Number.isInteger(null), false));
console.log(Object.is(Number.isInteger(Symbol("1")), false));
console.log(Object.is(Number.isInteger(), false));
