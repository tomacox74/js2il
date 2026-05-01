// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.1
description: >
    rest index
---*/
function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

assert.sameValue(
    (function(...args) { return args.length; })(1,2,3,4,5),
    5,
    "`(function(...args) { return args.length; })(1,2,3,4,5)` returns `5`"
);
assert.sameValue(
    (function(a, ...args) { return args.length; })(1,2,3,4,5),
    4,
    "`(function(a, ...args) { return args.length; })(1,2,3,4,5)` returns `4`"
);
assert.sameValue(
    (function(a, b, ...args) { return args.length; })(1,2,3,4,5),
    3,
    "`(function(a, b, ...args) { return args.length; })(1,2,3,4,5)` returns `3`"
);
assert.sameValue(
    (function(a, b, c, ...args) { return args.length; })(1,2,3,4,5),
    2,
    "`(function(a, b, c, ...args) { return args.length; })(1,2,3,4,5)` returns `2`"
);
assert.sameValue(
    (function(a, b, c, d, ...args) { return args.length; })(1,2,3,4,5),
    1,
    "`(function(a, b, c, d, ...args) { return args.length; })(1,2,3,4,5)` returns `1`"
);
assert.sameValue(
    (function(a, b, c, d, e, ...args) { return args.length; })(1,2,3,4,5),
    0,
    "`(function(a, b, c, d, e, ...args) { return args.length; })(1,2,3,4,5)` returns `0`"
);
