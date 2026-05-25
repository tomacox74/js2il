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
info: If ToBoolean(x) is true, return x
es5id: 11.11.2_A4_T2
description: Type(x) and Type(y) vary between primitive number and Number object
---*/

//CHECK#1
if ((-1 || 1) !== -1) {
  throw new Test262Error('#1: (-1 || 1) === -1');
}

//CHECK#2
if ((1 || new Number(0)) !== 1) {
  throw new Test262Error('#2: (1 || new Number(0)) === 1');
} 

//CHECK#3
if ((-1 || NaN) !== -1) {
  throw new Test262Error('#3: (-1 || NaN) === -1');
}

//CHECK#4
var x = new Number(-1);
if ((x || new Number(0)) !== x) {
  throw new Test262Error('#4: (var x = new Number(-1); (x || new Number(-1)) === x');
}

//CHECK#5
var x = new Number(NaN);
if ((x || new Number(1)) !== x) {
  throw new Test262Error('#5: (var x = new Number(NaN); (x || new Number(1)) === x');
}

//CHECK#6
var x = new Number(0);
if ((x || new Number(NaN)) !== x) {
  throw new Test262Error('#6: (var x = new Number(0); (x || new Number(NaN)) === x');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
