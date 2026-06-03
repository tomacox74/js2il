// Copyright (C) 2021 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findlastindex
description: >
  Return index if predicate return a boolean true value.
info: |
  Array.prototype.findLastIndex ( predicate[ , thisArg ] )

  ...
  5. Repeat, while k ≥ 0,
    ...
    c. Let testResult be ! ToBoolean(? Call(predicate, thisArg, « kValue, 𝔽(k), O »)).
    d. If testResult is true, return 𝔽(k).
  ...
features: [Symbol, array-find-from-last]
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
  if (actual == null || expected == null || actual.length !== expected.length) {
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

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}


var arr = ['Shoes', 'Car', 'Bike'];
var called = 0;

var result = arr.findLastIndex(function() {
  called++;
  return true;
});

assert.sameValue(result, 2);
assert.sameValue(called, 1, 'predicate was called once');

called = 0;
result = arr.findLastIndex(function(val) {
  called++;
  return val === 'Shoes';
});

assert.sameValue(called, 3, 'predicate was called three times');
assert.sameValue(result, 0);

result = arr.findLastIndex(function() {
  return 'string';
});
assert.sameValue(result, 2, 'coerced string');

result = arr.findLastIndex(function() {
  return {};
});
assert.sameValue(result, 2, 'coerced object');

result = arr.findLastIndex(function() {
  return Symbol('');
});
assert.sameValue(result, 2, 'coerced Symbol');

result = arr.findLastIndex(function() {
  return 1;
});
assert.sameValue(result, 2, 'coerced number');

result = arr.findLastIndex(function() {
  return -1;
});
assert.sameValue(result, 2, 'coerced negative number');
