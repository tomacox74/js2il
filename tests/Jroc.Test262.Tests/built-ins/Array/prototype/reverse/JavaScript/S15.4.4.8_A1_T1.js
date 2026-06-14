// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The elements of the array are rearranged so as to reverse their order.
    The object is returned as the result of the call
esid: sec-array.prototype.reverse
description: Checking case when reverse is given no arguments or one argument
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
var reverse = x.reverse();
if (reverse !== x) {
  throw new Test262Error('#1: x = []; x.reverse() === x. Actual: ' + (reverse));
}

x = [];
x[0] = 1;
var reverse = x.reverse();
if (reverse !== x) {
  throw new Test262Error('#2: x = []; x[0] = 1; x.reverse() === x. Actual: ' + (reverse));
}

x = new Array(1, 2);
var reverse = x.reverse();
if (reverse !== x) {
  throw new Test262Error('#3: x = new Array(1,2); x.reverse() === x. Actual: ' + (reverse));
}

if (x[0] !== 2) {
  throw new Test262Error('#4: x = new Array(1,2); x.reverse(); x[0] === 2. Actual: ' + (x[0]));
}

if (x[1] !== 1) {
  throw new Test262Error('#5: x = new Array(1,2); x.reverse(); x[1] === 1. Actual: ' + (x[1]));
}

if (x.length !== 2) {
  throw new Test262Error('#6: x = new Array(1,2); x.reverse(); x.length === 2. Actual: ' + (x.length));
}

console.log(true);
