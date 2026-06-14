// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.slice
description: >
    Array.prototype.slice will slice a string from start to end when
    index property (read-only) exists in Array.prototype (Step 10.c.ii)
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

var arrObj = [1, 2, 3];

Object.defineProperty(Array.prototype, "0", {
  value: "test",
  writable: false,
  configurable: true
});

var newArr = arrObj.slice(0, 1);

assert(newArr.hasOwnProperty("0"), 'newArr.hasOwnProperty("0") !== true');
assert.sameValue(newArr[0], 1, 'newArr[0]');
assert.sameValue(typeof newArr[1], "undefined", 'typeof newArr[1]');
