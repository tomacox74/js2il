// Copyright (C) 2016 The V8 Project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-isnan-number
description: >
  Use non-object value returned from @@toPrimitive method
info: |
  isNaN (number)

  1. Let num be ? ToNumber(number).

  ToPrimitive ( input [ , PreferredType ] )

  [...]
  4. Let exoticToPrim be ? GetMethod(input, @@toPrimitive).
  5. If exoticToPrim is not undefined, then
    a. Let result be ? Call(exoticToPrim, input, « hint »).
    b. If Type(result) is not Object, return result.
features: [Symbol.toPrimitive]
---*/

var called = 0;
var obj = {
  valueOf: function() {
    called = NaN;
    return Infinity;
  },
  toString: function() {
    called = NaN;
    return Infinity;
  }
};

obj[Symbol.toPrimitive] = function() {
  called += 1;
  return 42;
};
console.log(Object.is(isNaN(obj), false));
console.log(Object.is(called, 1));

called = 0;
obj[Symbol.toPrimitive] = function() {
  called += 1;
  return "this is not a number";
};
console.log(Object.is(isNaN(obj), true));
console.log(Object.is(called, 1));

called = 0;
obj[Symbol.toPrimitive] = function() {
  called += 1;
  return true;
};
console.log(Object.is(isNaN(obj), false));
console.log(Object.is(called, 1));

called = 0;
obj[Symbol.toPrimitive] = function() {
  called += 1;
  return false;
};
console.log(Object.is(isNaN(obj), false));
console.log(Object.is(called, 1));

called = 0;
obj[Symbol.toPrimitive] = function() {
  called += 1;
  return NaN;
};
console.log(Object.is(isNaN(obj), true));
console.log(Object.is(called, 1));
