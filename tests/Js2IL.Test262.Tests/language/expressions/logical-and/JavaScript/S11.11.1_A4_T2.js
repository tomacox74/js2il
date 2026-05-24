function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
var assert = function assert(value, message) {
    if (!value) {
        throw new Test262Error(message || "Assertion failed");
    }
};
assert.sameValue = function(actual, expected, message) {
    if (!Object.is(actual, expected)) {
        throw new Test262Error(message || "Expected SameValue");
    }
};
assert.notSameValue = function(actual, unexpected, message) {
    if (Object.is(actual, unexpected)) {
        throw new Test262Error(message || "Expected different values");
    }
};
assert.throws = function(expectedErrorConstructor, func, message) {
    try {
        func();
    } catch (error) {
        if (error instanceof expectedErrorConstructor || error.constructor === expectedErrorConstructor) {
            return;
        }
        throw new Test262Error(message || "Unexpected error type");
    }
    throw new Test262Error(message || "Expected function to throw");
};
assert.compareArray = function(actual, expected, message) {
    if (actual.length !== expected.length || !actual.every(function(value, index) { return Object.is(value, expected[index]); })) {
        throw new Test262Error(message || "Expected arrays to match");
    }
};

try {
// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If ToBoolean(x) is true, return y
es5id: 11.11.1_A4_T2
description: Type(x) and Type(y) vary between primitive number and Number object
---*/

//CHECK#1
if ((-1 && -0) !== 0) {
  throw new Test262Error('#1.1: (-1 && -0) === 0');
} else {
  if ((1 / (-1 && -0)) !== Number.NEGATIVE_INFINITY) {
    throw new Test262Error('#1.2: (-1 && -0) === -0');
  }
}

//CHECK#2
if ((-1 && 0) !== 0) {
  throw new Test262Error('#2.1: (-1 && 0) === 0');
} else {
  if ((1 / (-1 && 0)) !== Number.POSITIVE_INFINITY) {
    throw new Test262Error('#2.2: (-1 && 0) === +0');
  }
}

//CHECK#3
if ((isNaN(0.1 && NaN)) !== true) {
  throw new Test262Error('#3: (0.1 && NaN) === Not-a-Number');
}

//CHECK#4
var y = new Number(0);
if ((new Number(-1) && y) !== y) {
  throw new Test262Error('#4: (var y = new Number(0); (new Number(-1) && y) === y');
}

//CHECK#5
var y = new Number(NaN);
if ((new Number(0) && y) !== y) {
  throw new Test262Error('#5: (var y = new Number(NaN); (new Number(0) && y) === y');
}

//CHECK#6
var y = new Number(-1);
if ((new Number(NaN) && y) !== y) {
  throw new Test262Error('#6: (var y = new Number(-1); (new Number(NaN) && y) === y');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
