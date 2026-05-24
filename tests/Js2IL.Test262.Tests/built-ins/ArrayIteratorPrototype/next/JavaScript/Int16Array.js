// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-%arrayiteratorprototype%.next
description: >
    Visits each element of the array in order and ceases iteration once all
    values have been visited.
features: [Symbol.iterator, TypedArray]
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

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
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

var array = new Int16Array([3, 1, 2]);
var iterator = array[Symbol.iterator]();
var result;

result = iterator.next();
assert.sameValue(result.value, 3, 'first result `value`');
assert.sameValue(result.done, false, 'first result `done` flag');

result = iterator.next();
assert.sameValue(result.value, 1, 'second result `value`');
assert.sameValue(result.done, false, 'second result `done` flag');

result = iterator.next();
assert.sameValue(result.value, 2, 'third result `value`');
assert.sameValue(result.done, false, 'third result `done` flag');

result = iterator.next();
assert.sameValue(result.value, undefined, 'exhausted result `value`');
assert.sameValue(result.done, true, 'exhausted result `done` flag');

result = iterator.next();
assert.sameValue(
  result.value, undefined, 'exhausted result `value` (repeated request)'
);
assert.sameValue(
  result.done, true, 'exhausted result `done` flag (repeated request)'
);
