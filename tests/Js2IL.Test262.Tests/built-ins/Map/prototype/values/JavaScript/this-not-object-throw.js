// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-map.prototype.values
description: >
  Throws a TypeError if `this` is not an Object.
info: |
  Map.prototype.values ()

  ...
  2. Return CreateMapIterator(M, "values").

  23.1.5.1 CreateMapIterator Abstract Operation

  1. If Type(map) is not Object, throw a TypeError exception.
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

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (var i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};

function verifyProperty(obj, name, desc) {
  var originalDesc = Object.getOwnPropertyDescriptor(obj, name);
  console.log(!!originalDesc);
  if ('value' in desc) {
    console.log(Object.is(originalDesc.value, desc.value));
  }
  if ('writable' in desc) {
    console.log(originalDesc.writable === desc.writable);
  }
  if ('enumerable' in desc) {
    console.log(originalDesc.enumerable === desc.enumerable);
  }
  if ('configurable' in desc) {
    console.log(originalDesc.configurable === desc.configurable);
  }
  if ('get' in desc) {
    console.log(Object.is(originalDesc.get, desc.get));
  }
  if ('set' in desc) {
    console.log(Object.is(originalDesc.set, desc.set));
  }
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;


assert.throws(TypeError, function() {
  Map.prototype.values.call(false);
});

assert.throws(TypeError, function() {
  Map.prototype.values.call(1);
});

assert.throws(TypeError, function() {
  Map.prototype.values.call('');
});

assert.throws(TypeError, function() {
  Map.prototype.values.call(undefined);
});

assert.throws(TypeError, function() {
  Map.prototype.values.call(null);
});

assert.throws(TypeError, function() {
  Map.prototype.values.call(Symbol());
});

assert.throws(TypeError, function() {
  var map = new Map();
  map.values.call(false);
});
