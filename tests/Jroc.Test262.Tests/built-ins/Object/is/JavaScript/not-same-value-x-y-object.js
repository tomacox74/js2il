// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 19.1.2.10
description: >
    Object.is ( value1, value2 )

    ...
    10. Return true if x and y are the same Object value. Otherwise, return false.
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

assert.sameValue(Object.is({}, {}), false, "`Object.is({}, {})` returns `false`");
assert.sameValue(
  Object.is(Object(), Object()),
  false,
  "`Object.is(Object(), Object())` returns `false`"
);
assert.sameValue(
  Object.is(new Object(), new Object()),
  false,
  "`Object.is(new Object(), new Object())` returns `false`"
);
assert.sameValue(
  Object.is(Object(0), Object(0)),
  false,
  "`Object.is(Object(0), Object(0))` returns `false`"
);
assert.sameValue(
  Object.is(new Object(''), new Object('')),
  false,
  "`Object.is(new Object(''), new Object(''))` returns `false`"
);
