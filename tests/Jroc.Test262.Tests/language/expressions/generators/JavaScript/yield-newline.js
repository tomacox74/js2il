// Copyright (C) 2013 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: >
    Newlines terminate `yield` expressions.
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

var iter, result;
var g = function*() {
  yield
  1
};

iter = g();

result = iter.next();
assert.sameValue(result.value, undefined, 'First result `value`');
assert.sameValue(result.done, false, 'First result `done` flag');

result = iter.next();
assert.sameValue(result.value, undefined, 'Second result `value`');
assert.sameValue(result.done, true, 'Second result `done` flag');
