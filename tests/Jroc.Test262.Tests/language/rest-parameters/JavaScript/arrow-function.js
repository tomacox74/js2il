// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.1
description: >
    arrow functions
includes: [compareArray.js]
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

var fn = (a, b, ...c) => c;

assert.compareArray(fn(), []);
assert.compareArray(fn(1, 2), []);
assert.compareArray(fn(1, 2, 3), [3]);
assert.compareArray(fn(1, 2, 3, 4), [3, 4]);
assert.compareArray(fn(1, 2, 3, 4, 5), [3, 4, 5]);
assert.compareArray(((...args) => args)(), []);
assert.compareArray(((...args) => args)(1,2,3), [1,2,3]);
