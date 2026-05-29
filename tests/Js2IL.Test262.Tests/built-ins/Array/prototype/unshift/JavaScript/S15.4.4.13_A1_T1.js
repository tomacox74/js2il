// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The arguments are prepended to the start of the array, such that
    their order within the array is the same as the order in which they appear in
    the argument list
esid: sec-array.prototype.unshift
description: Checking case when unsift is given no arguments or one argument
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
var unshift = x.unshift(1);
if (unshift !== 1) {
  throw new Test262Error('#1: x = new Array(); x.unshift(1) === 1. Actual: ' + (unshift));
}

if (x[0] !== 1) {
  throw new Test262Error('#2: x = new Array(); x.unshift(1); x[0] === 1. Actual: ' + (x[0]));
}

var unshift = x.unshift();
if (unshift !== 1) {
  throw new Test262Error('#3: x = new Array(); x.unshift(1); x.unshift() === 1. Actual: ' + (unshift));
}

if (x[1] !== undefined) {
  throw new Test262Error('#4: x = new Array(); x.unshift(1); x.unshift(); x[1] === unedfined. Actual: ' + (x[1]));
}

var unshift = x.unshift(-1);
if (unshift !== 2) {
  throw new Test262Error('#5: x = new Array(); x.unshift(1); x.unshift(); x.unshift(-1) === 2. Actual: ' + (unshift));
}

if (x[0] !== -1) {
  throw new Test262Error('#6: x = new Array(); x.unshift(1); x.unshift(-1); x[0] === -1. Actual: ' + (x[0]));
}

if (x[1] !== 1) {
  throw new Test262Error('#7: x = new Array(); x.unshift(1); x.unshift(-1); x[1] === 1. Actual: ' + (x[1]));
}

if (x.length !== 2) {
  throw new Test262Error('#8: x = new Array(); x.unshift(1); x.unshift(); x.unshift(-1); x.length === 2. Actual: ' + (x.length));
}
