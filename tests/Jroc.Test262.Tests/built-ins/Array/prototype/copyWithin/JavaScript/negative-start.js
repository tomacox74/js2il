// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.copywithin
description: >
  Set values with negative start argument.
info: |
  22.1.3.3 Array.prototype.copyWithin (target, start [ , end ] )

  ...
  10. If relativeStart < 0, let from be max((len + relativeStart),0); else let
  from be min(relativeStart, len).
  ...
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


assert.compareArray(
  [0, 1, 2, 3].copyWithin(0, -1), [3, 1, 2, 3],
  '[0, 1, 2, 3].copyWithin(0, -1) must return [3, 1, 2, 3]'
);

assert.compareArray(
  [0, 1, 2, 3, 4].copyWithin(2, -2), [0, 1, 3, 4, 4],
  '[0, 1, 2, 3, 4].copyWithin(2, -2) must return [0, 1, 3, 4, 4]'
);

assert.compareArray(
  [0, 1, 2, 3, 4].copyWithin(1, -2), [0, 3, 4, 3, 4],
  '[0, 1, 2, 3, 4].copyWithin(1, -2) must return [0, 3, 4, 3, 4]'
);

assert.compareArray(
  [0, 1, 2, 3].copyWithin(-1, -2), [0, 1, 2, 2],
  '[0, 1, 2, 3].copyWithin(-1, -2) must return [0, 1, 2, 2]'
);

assert.compareArray(
  [0, 1, 2, 3, 4].copyWithin(-2, -3), [0, 1, 2, 2, 3],
  '[0, 1, 2, 3, 4].copyWithin(-2, -3) must return [0, 1, 2, 2, 3]'
);

assert.compareArray(
  [0, 1, 2, 3, 4].copyWithin(-5, -2), [3, 4, 2, 3, 4],
  '[0, 1, 2, 3, 4].copyWithin(-5, -2) must return [3, 4, 2, 3, 4]'
);
