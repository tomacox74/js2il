// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 19.1.2.10
description: >
    Object.is ( value1, value2 )

    ...
    6. If Type(x) is Number, then
      a. If x is NaN and y is NaN, return true.
      b. If x is +0 and y is -0, return false.
      c. If x is -0 and y is +0, return false.
      d. If x is the same Number value as y, return true.
      e. Return false.
    ...
---*/


// test262 execution-port helpers
function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

assert.sameValue(Object.is(+0, -0), false, "`Object.is(+0, -0)` returns `false`");
assert.sameValue(Object.is(-0, +0), false, "`Object.is(-0, +0)` returns `false`");
assert.sameValue(Object.is(0), false, "`Object.is(0)` returns `false`");
assert.sameValue(Object.is(Infinity, -Infinity), false, "`Object.is(Infinity, -Infinity)` returns `false`");
