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
info: If ToBoolean(x) is false, return z
es5id: 11.12_A3_T2
description: Type(y) and Type(z) are number primitives
---*/

//CHECK#1
if ((0 ? 0 : 1) !== 1) {
  throw new Test262Error('#1: (0 ? 0 : 1) === 1');
}

//CHECK#2
var z = new Number(1);
if ((0 ? 1 : z) !== z) {
  throw new Test262Error('#2: (var y = new Number(1); (0 ? 1 : z) === z');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
