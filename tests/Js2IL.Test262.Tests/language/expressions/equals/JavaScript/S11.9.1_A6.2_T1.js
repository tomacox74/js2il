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
info: If one expression is undefined or null and another is not, return false
es5id: 11.9.1_A6.2_T1
description: x is null or undefined, y is not
---*/

//CHECK#1
if ((undefined == true) !== false) {
  throw new Test262Error('#1: (undefined == true) === false');
}

//CHECK#2
if ((undefined == 0) !== false) {
  throw new Test262Error('#2: (undefined == 0) === false');
}

//CHECK#3
if ((undefined == "undefined") !== false) {
  throw new Test262Error('#3: (undefined == "undefined") === false');
}

//CHECK#4
if ((undefined == {}) !== false) {
  throw new Test262Error('#4: (undefined == {}) === false');
}

//CHECK#5
if ((null == false) !== false) {
  throw new Test262Error('#5: (null == false) === false');
}

//CHECK#6
if ((null == 0) !== false) {
  throw new Test262Error('#6: (null == 0) === false');
}

//CHECK#7
if ((null == "null") !== false) {
  throw new Test262Error('#7: (null == "null") === false');
}

//CHECK#8
if ((null == {}) !== false) {
  throw new Test262Error('#8: (null == {}) === false');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
