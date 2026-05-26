// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    If length equal zero, call the [[Put]] method of this object
    with arguments "length" and 0 and return undefined
esid: sec-array.prototype.shift
description: Checking this algorithm
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

var x = new Array();
var shift = x.shift();
if (shift !== undefined) {
  throw new Test262Error('#1: var x = new Array(); x.shift() === undefined. Actual: ' + (shift));
}

if (x.length !== 0) {
  throw new Test262Error('#2: var x = new Array(); x.shift(); x.length === 0. Actual: ' + (x.length));
}

var x = Array(1, 2, 3);
x.length = 0;
var shift = x.shift();
if (shift !== undefined) {
  throw new Test262Error('#2: var x = Array(1,2,3); x.length = 0; x.shift() === undefined. Actual: ' + (shift));
}

if (x.length !== 0) {
  throw new Test262Error('#4: var x = new Array(1,2,3); x.length = 0; x.shift(); x.length === 0. Actual: ' + (x.length));
}
