// Copyright (C) 2018 Shilpi Jain and Michael Ficarra. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-array.prototype.flat
description: >
    if the argument is a string or object, the depthNum is 0
includes: [compareArray.js]
features: [Array.prototype.flat]
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

function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  if (actual === undefined) {
    console.log(false);
    return;
  }

  var ok = true;
  if (Object.prototype.hasOwnProperty.call(desc, 'value')) ok = ok && Object.is(actual.value, desc.value);
  if (Object.prototype.hasOwnProperty.call(desc, 'writable')) ok = ok && Object.is(actual.writable, desc.writable);
  if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) ok = ok && Object.is(actual.enumerable, desc.enumerable);
  if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) ok = ok && Object.is(actual.configurable, desc.configurable);
  console.log(ok);
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;


var a = [1, [2]];
var expected = a;

// non integral string depthNum is converted to 0
var depthNum = 'TestString';
var actual = a.flat(depthNum);
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// object type depthNum is converted to 0
depthNum = {};
actual = a.flat(depthNum);
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// negative infinity depthNum is converted to 0
depthNum = Number.NEGATIVE_INFINITY;
actual = a.flat(depthNum);
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// positive zero depthNum is converted to 0
depthNum = +0;
actual = a.flat(depthNum);
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// negative zero depthNum is converted to 0
depthNum = -0;
actual = a.flat(depthNum);
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// integral string depthNum is converted to an integer
depthNum = '1';
actual = a.flat(depthNum);
expected = [1, 2]
assert.compareArray(actual, expected, 'The value of actual is expected to equal the value of expected');

// undefined depthNum uses the default value of 1
actual = a.flat(undefined);
expected = [1, 2];
assert.compareArray(actual, expected, 'a.flat(undefined) uses default depth of 1');
