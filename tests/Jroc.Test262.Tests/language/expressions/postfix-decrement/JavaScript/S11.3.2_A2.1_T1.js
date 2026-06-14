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
info: Operator x-- uses GetValue and PutValue
es5id: 11.3.2_A2.1_T1
description: Type(x) is Reference and GetBase(x) is not null
---*/

//CHECK#1
var x = 1;
if (x-- !== 1) {
  throw new Test262Error('#1: var x = 1; x-- === 1. Actual: ' + (x--));
} else {
  if (x !== 1 - 1) {
    throw new Test262Error('#1: var x = 1; x--; x === 1 - 1. Actual: ' + (x));
  } 
}

//CHECK#2
this.x = 1;
if (this.x-- !== 1) {
  throw new Test262Error('#2: this.x = 1; this.x-- === 1. Actual: ' + (this.x--));
} else {
  if (this.x !== 1 - 1) {
    throw new Test262Error('#2: this.x = 1; this.x--; this.x === 1 - 1. Actual: ' + (this.x));
  } 
}

//CHECK#3
var object = new Object();
object.prop = 1;
if (object.prop-- !== 1) {
  throw new Test262Error('#3: var object = new Object(); object.prop = 1; object.prop-- === 1. Actual: ' + (object.prop--));
} else {
  if (this.x !== 1 - 1) {
    throw new Test262Error('#3: var object = new Object(); object.prop = 1; object.prop--; object.prop === 1 - 1. Actual: ' + (object.prop));
  } 
}

    console.log(true);
} catch (error) {
    console.log(false);
}
