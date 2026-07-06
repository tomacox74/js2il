// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: >
    Generator objects should define a `length` property.
includes: [propertyHelper.js]
es6id: 25.2.4
features: [generators]
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

var g = function*() {};

verifyProperty(g, "length", {
  value: 0,
  writable: false,
  enumerable: false,
  configurable: true,
});
