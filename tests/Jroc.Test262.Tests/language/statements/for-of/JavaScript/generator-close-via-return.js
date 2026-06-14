// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.6.4.13
description: >
    Generators should be closed via their `return` method when iteration is
    interrupted via a `return` statement.
features: [generators]
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

var startedCount = 0;
var finallyCount = 0;
var iterationCount = 0;
function* values() {
  startedCount += 1;
  try {
    yield;
    throw new Test262Error('This code is unreachable (within `try` block)');
  } finally {
    finallyCount += 1;
  }
  throw new Test262Error('This code is unreachable (following `try` statement)');
}
var iterable = values();

assert.sameValue(
  startedCount, 0, 'Generator is initialized in suspended state'
);

(function() {
  for (var x of iterable) {
    assert.sameValue(
      startedCount, 1, 'Generator executes prior to first iteration'
    );
    assert.sameValue(
      finallyCount, 0, 'Generator is paused during first iteration'
    );
    iterationCount += 1;
    return;
  }
}());

assert.sameValue(
  startedCount, 1, 'Generator does not restart following interruption'
);
assert.sameValue(iterationCount, 1, 'A single iteration occurs');
assert.sameValue(
  finallyCount, 1, 'Generator is closed after `return` statement'
);
