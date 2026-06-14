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
info: Operator ++x returns x = ToNumber(x) + 1
es5id: 11.4.4_A3_T1
description: Type(x) is boolean primitive or Boolean object
---*/

//CHECK#1
var x = false; 
++x;
if (x !== 0 + 1) {
  throw new Test262Error('#1: var x = false; ++x; x === 0 + 1. Actual: ' + (x));
}

//CHECK#2
var x = new Boolean(true); 
++x; 
if (x !== 1 + 1) {
  throw new Test262Error('#2: var x = new Boolean(true); ++x; x === 1 + 1. Actual: ' + (x));
}

    console.log(true);
} catch (error) {
    console.log(false);
}
