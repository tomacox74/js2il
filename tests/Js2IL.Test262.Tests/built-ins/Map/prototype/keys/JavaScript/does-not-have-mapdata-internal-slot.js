// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-map.prototype.keys
description: >
  Throws a TypeError if `this` object does not have a [[MapData]] internal slot.
info: |
  Map.prototype.keys ()

  1. Let M be the this value.
  2. Return CreateMapIterator(M, "key").

  23.1.5.1 CreateMapIterator Abstract Operation

  ...
  2. If map does not have a [[MapData]] internal slot, throw a TypeError
  exception.
  ...

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


var m = new Map();

assert.throws(TypeError, function() {
  Map.prototype.keys.call([]);
});

assert.throws(TypeError, function() {
  m.keys.call([]);
});

assert.throws(TypeError, function() {
  Map.prototype.keys.call({});
});

assert.throws(TypeError, function() {
  m.keys.call({});
});
