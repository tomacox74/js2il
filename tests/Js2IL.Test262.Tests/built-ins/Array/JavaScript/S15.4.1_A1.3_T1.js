// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    This description of Array constructor applies if and only if
    the Array constructor is given no arguments or at least two arguments
es5id: 15.4.1_A1.3_T1
description: Checking case when Array constructor is given one argument
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

var x = Array(2);

assert.notSameValue(x.length, 1, 'The value of x.length is not 1');
assert.notSameValue(x[0], 2, 'The value of x[0] is not 2');
