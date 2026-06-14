// Copyright (C) 2021 Microsoft. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.findlast
description: >
  Return undefined if predicate always returns a boolean false value.
info: |
  Array.prototype.findLast ( predicate[ , thisArg ] )

  ...
  5. Repeat, while k ≥ 0,
    ...
    c. Let testResult be ! ToBoolean(? Call(predicate, thisArg, « kValue, 𝔽(k), O »)).
    ...
  6. Return undefined.
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

var result = arr.findLast(function() {
  called++;
  return false;
});

assert.sameValue(called, 3, 'predicate was called three times');
assert.sameValue(result, undefined);

result = arr.findLast(function() {
  return '';
});
assert.sameValue(result, undefined, 'coerced string');

result = arr.findLast(function() {
  return undefined;
});
assert.sameValue(result, undefined, 'coerced undefined');

result = arr.findLast(function() {
  return null;
});
assert.sameValue(result, undefined, 'coerced null');

result = arr.findLast(function() {
  return 0;
});
assert.sameValue(result, undefined, 'coerced 0');

result = arr.findLast(function() {
  return NaN;
});
assert.sameValue(result, undefined, 'coerced NaN');
