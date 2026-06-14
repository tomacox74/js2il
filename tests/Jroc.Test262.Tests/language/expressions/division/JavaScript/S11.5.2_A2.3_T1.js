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
    ToNumber(first expression) is called first, and then ToNumber(second
    expression)
es5id: 11.5.2_A2.3_T1
description: Checking with "throw"
---*/

//CHECK#1
var x = { valueOf: function () { throw "x"; } };
var y = { valueOf: function () { throw "y"; } };
try {
   x / y;
   throw new Test262Error('#1.1: var x = { valueOf: function () { throw "x"; } }; var y = { valueOf: function () { throw "y"; } }; x / y throw "x". Actual: ' + (x / y));
} catch (e) {
   if (e === "y") {
     throw new Test262Error('#1.2: ToNumber(first expression) is called first, and then ToNumber(second expression)');
   } else {
     if (e !== "x") {
       throw new Test262Error('#1.3: var x = { valueOf: function () { throw "x"; } }; var y = { valueOf: function () { throw "y"; } }; x / y throw "x". Actual: ' + (e));
     }
   }
}

    console.log(true);
} catch (error) {
    console.log(false);
}
