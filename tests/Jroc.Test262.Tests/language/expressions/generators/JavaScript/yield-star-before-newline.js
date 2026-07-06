// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: >
    The right-hand side of a `yield *` expression may appear on a new line.
es6id: 14.4
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

var result;
var g = function*() {
  yield *
  g2();
};
var g2 = function*() {};

result = g().next();
assert.sameValue(result.value, undefined);
assert.sameValue(result.done, true);
