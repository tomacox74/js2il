// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findindex
description: >
  Return index if predicate return a boolean true value.
info: |
  22.1.3.9 Array.prototype.findIndex ( predicate[ , thisArg ] )

  ...
  8. Repeat, while k < len
    ...
    d. Let testResult be ToBoolean(Call(predicate, T, «kValue, k, O»)).
    e. ReturnIfAbrupt(testResult).
    f. If testResult is true, return k.
  ...
features: [Symbol]
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

var arr = ['Shoes', 'Car', 'Bike'];
var called = 0;

var result = arr.findIndex(function(val) {
  called++;
  return true;
});

assert.sameValue(result, 0);
assert.sameValue(called, 1, 'predicate was called once');

called = 0;
result = arr.findIndex(function(val) {
  called++;
  return val === 'Bike';
});

assert.sameValue(called, 3, 'predicate was called three times');
assert.sameValue(result, 2);

result = arr.findIndex(function(val) {
  return 'string';
});
assert.sameValue(result, 0, 'coerced string');

result = arr.findIndex(function(val) {
  return {};
});
assert.sameValue(result, 0, 'coerced object');

result = arr.findIndex(function(val) {
  return Symbol('');
});
assert.sameValue(result, 0, 'coerced Symbol');

result = arr.findIndex(function(val) {
  return 1;
});
assert.sameValue(result, 0, 'coerced number');

result = arr.findIndex(function(val) {
  return -1;
});
assert.sameValue(result, 0, 'coerced negative number');
