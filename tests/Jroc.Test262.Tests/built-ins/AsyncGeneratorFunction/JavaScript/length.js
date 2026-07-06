// Copyright (C) 2018 Valerie Young. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-asyncgeneratorfunction-length
description: >
  This is a data property with a value of 1. This property has the attributes
  { [[Writable]]: false, [[Enumerable]]: false, [[Configurable]]: true }.
includes: [propertyHelper.js]
features: [async-iteration]
---*/

function __sameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __assertResult(passed, message) {
  console.log(!!passed);
  if (!passed) {
    throw new Error(message || 'Assertion failed');
  }
}

function checkSequence(sequence, message) {
  var passed = true;
  for (var i = 0; i < sequence.length; i++) {
    if (sequence[i] !== i + 1) {
      passed = false;
      break;
    }
  }
  __assertResult(passed, message || 'Unexpected callback sequence');
}

var AsyncGeneratorFunction = Object.getPrototypeOf(async function* () {}).constructor;

verifyProperty(AsyncGeneratorFunction, "length", {
  value: 1,
  enumerable: false,
  writable: false,
  configurable: true,
});
