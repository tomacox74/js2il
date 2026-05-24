// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The last element of the array is removed from the array
    and returned
esid: sec-array.prototype.pop
description: Checking this use new Array() and []
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

var x = new Array(0, 1, 2, 3);
var pop = x.pop();
if (pop !== 3) {
  throw new Test262Error('#1: x = new Array(0,1,2,3); x.pop() === 3. Actual: ' + (pop));
}

if (x.length !== 3) {
  throw new Test262Error('#2: x = new Array(0,1,2,3); x.pop(); x.length == 3');
}

if (x[3] !== undefined) {
  throw new Test262Error('#3: x = new Array(0,1,2,3); x.pop(); x[3] == undefined');
}

if (x[2] !== 2) {
  throw new Test262Error('#4: x = new Array(0,1,2,3); x.pop(); x[2] == 2');
}

x = [];
x[0] = 0;
x[3] = 3;
var pop = x.pop();
if (pop !== 3) {
  throw new Test262Error('#5: x = []; x[0] = 0; x[3] = 3; x.pop() === 3. Actual: ' + (pop));
}

if (x.length !== 3) {
  throw new Test262Error('#6: x = []; x[0] = 0; x[3] = 3; x.pop(); x.length == 3');
}

if (x[3] !== undefined) {
  throw new Test262Error('#7: x = []; x[0] = 0; x[3] = 3; x.pop(); x[3] == undefined');
}

if (x[2] !== undefined) {
  throw new Test262Error('#8: x = []; x[0] = 0; x[3] = 3; x.pop(); x[2] == undefined');
}

x.length = 1;
var pop = x.pop();
if (pop !== 0) {
  throw new Test262Error('#9: x = []; x[0] = 0; x[3] = 3; x.pop(); x.length = 1; x.pop() === 0. Actual: ' + (pop));
}

if (x.length !== 0) {
  throw new Test262Error('#10: x = []; x[0] = 0; x[3] = 3; x.pop(); x.length = 1; x.pop(); x.length === 0. Actual: ' + (x.length));
}

console.log(true);
