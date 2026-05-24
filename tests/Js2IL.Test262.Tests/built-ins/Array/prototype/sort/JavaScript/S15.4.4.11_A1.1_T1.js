// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    If this object does not have a property named by ToString(j),
    and this object does not have a property named by ToString(k), return +0
esid: sec-array.prototype.sort
description: If comparefn is undefined, use SortCompare operator
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

var x = new Array(2);
x.sort();

if (x.length !== 2) {
  throw new Test262Error('#1: var x = new Array(2); x.sort(); x.length === 2. Actual: ' + (x.length));
}

if (x[0] !== undefined) {
  throw new Test262Error('#2: var x = new Array(2); x.sort(); x[0] === undefined. Actual: ' + (x[0]));
}

if (x[1] !== undefined) {
  throw new Test262Error('#3: var x = new Array(2); x.sort(); x[1] === undefined. Actual: ' + (x[1]));
}

console.log(true);
