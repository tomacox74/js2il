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
info: |
    Type(x) and Type(y) are String-s.
    Return true, if x and y are exactly the same sequence of characters; otherwise, return false
es5id: 11.9.1_A5.1
description: x and y are primitive string
---*/

//CHECK#1
if (("" == "") !== true) {
  throw new Test262Error('#1: ("" == "") === true');
}

//CHECK#2
if ((" " == " ") !== true) {
  throw new Test262Error('#2: " (" == " ") === true');
}

//CHECK#3
if ((" " == "") !== false) {
  throw new Test262Error('#3: " (" == "") === false');
}

//CHECK#4
if (("string" == "string") !== true) {
  throw new Test262Error('#4: ("string" == "string") === true');
}

//CHECK#5
if ((" string" == "string ") !== false) {
  throw new Test262Error('#5: (" string" == "string ") === false');
}

//CHECK#6
if (("1.0" == "1") !== false) {
  throw new Test262Error('#6: ("1.0" == "1") === false');
}

//CHECK#7
if (("0xff" == "255") !== false) {
  throw new Test262Error('#7: ("0xff" == "255") === false');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
