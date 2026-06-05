// Copyright (C) 2021 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findlast
description: >
  The range of elements processed is set before the first call to `predicate`.
info: |
  Array.prototype.findLast ( predicate[ , thisArg ] )

  ...
  4. Let k be len - 1.
  5. Repeat, while k ≥ 0,
  ...
  c. Let testResult be ! ToBoolean(? Call(predicate, thisArg, « kValue, 𝔽(k), O »)).
  ...
features: [array-find-from-last]
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
var results = [];

arr.findLast(function(kValue) {
  if (results.length === 0) {
    arr.splice(1, 1);
  }
  results.push(kValue);
});

assert.sameValue(results.length, 3, 'predicate called three times');
assert.sameValue(results[0], 'Bike');
assert.sameValue(results[1], 'Bike');
assert.sameValue(results[2], 'Shoes');

results = [];
arr = ['Skateboard', 'Barefoot'];
arr.findLast(function(kValue) {
  if (results.length === 0) {
    arr.push('Motorcycle');
    arr[0] = 'Magic Carpet';
  }

  results.push(kValue);
});

assert.sameValue(results.length, 2, 'predicate called twice');
assert.sameValue(results[0], 'Barefoot');
assert.sameValue(results[1], 'Magic Carpet');
