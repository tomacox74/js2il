// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Every Array object has a length property whose value is
    always a nonnegative integer less than 2^32. The value of the length property is
    numerically greater than the name of every property whose name is an array index
es5id: 15.4.5.2_A1_T1
description: Checking boundary points
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

var x = [];
assert.sameValue(x.length, 0, 'The value of x.length is expected to be 0');

x[0] = 1;
assert.sameValue(x.length, 1, 'The value of x.length is expected to be 1');

x[1] = 1;
assert.sameValue(x.length, 2, 'The value of x.length is expected to be 2');

x[2147483648] = 1;
assert.sameValue(x.length, 2147483649, 'The value of x.length is expected to be 2147483649');

x[4294967294] = 1;
assert.sameValue(x.length, 4294967295, 'The value of x.length is expected to be 4294967295');
