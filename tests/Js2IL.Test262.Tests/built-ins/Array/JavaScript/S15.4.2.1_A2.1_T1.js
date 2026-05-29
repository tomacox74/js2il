// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The length property of the newly constructed object;
    is set to the number of arguments
es5id: 15.4.2.1_A2.1_T1
description: Array constructor is given no arguments or at least two arguments
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

assert.sameValue(new Array().length, 0, 'The value of new Array().length is expected to be 0');
assert.sameValue(new Array(0, 1, 0, 1).length, 4, 'The value of new Array(0, 1, 0, 1).length is expected to be 4');

assert.sameValue(
  new Array(undefined, undefined).length,
  2,
  'The value of new Array(undefined, undefined).length is expected to be 2'
);
