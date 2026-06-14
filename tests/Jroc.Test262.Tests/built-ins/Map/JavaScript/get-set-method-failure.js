// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-map-iterable
description: >
  new Map returns abrupt from getting Map.prototype.set.
info: |
  Map ( [ iterable ] )

  ...
  7. Else,
    a. Let adder be Get(map, "add").
    b. ReturnIfAbrupt(adder).
---*/

function Test262Error(message) {
  this.name = "Test262Error";
  this.message = message || "";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };
assert.compareArray = function(actual, expected) {
  var sameLength = actual.length === expected.length;
  var sameValues = sameLength;
  for (var i = 0; i < actual.length && sameValues; i++) {
    sameValues = Object.is(actual[i], expected[i]);
  }
  console.log(sameLength && sameValues);
};
assert.throws = function(expectedErrorConstructor, func) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};
function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  console.log(actual !== undefined);
  if ('value' in desc) {
    console.log(Object.is(actual && actual.value, desc.value));
  }
  if ('writable' in desc) {
    console.log(!!actual && actual.writable === desc.writable);
  }
  if ('enumerable' in desc) {
    console.log(!!actual && actual.enumerable === desc.enumerable);
  }
  if ('configurable' in desc) {
    console.log(!!actual && actual.configurable === desc.configurable);
  }
}
function isConstructor(argument) {
  if ((typeof argument !== 'function' && typeof argument !== 'object') || argument === null) {
    return false;
  }
  try {
    Reflect.construct(function() {}, [], argument);
    return true;
  } catch (error) {
    return false;
  }
}

Object.defineProperty(Map.prototype, 'set', {
  get: function() {
    throw new Test262Error();
  }
});

new Map();

assert.throws(Test262Error, function() {
  new Map([]);
});
