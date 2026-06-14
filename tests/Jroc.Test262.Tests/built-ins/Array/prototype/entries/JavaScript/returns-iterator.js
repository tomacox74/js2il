// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.entries
description: >
  The method should return an Iterator instance.
info: |
  22.1.3.4 Array.prototype.entries ( )

  1. Let O be ToObject(this value).
  2. ReturnIfAbrupt(O).
  3. Return CreateArrayIterator(O, "key+value").

  22.1.5.1 CreateArrayIterator Abstract Operation

  ...
  2. Let iterator be ObjectCreate(%ArrayIteratorPrototype%, «‍[[IteratedObject]],
  [[ArrayIteratorNextIndex]], [[ArrayIterationKind]]»).
  ...
  6. Return iterator.
features: [Symbol.iterator]
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

var ArrayIteratorProto = Object.getPrototypeOf([][Symbol.iterator]());
var iter = [].entries();

assert.sameValue(
  Object.getPrototypeOf(iter), ArrayIteratorProto,
  'The prototype of [].entries() is %ArrayIteratorPrototype%'
);
