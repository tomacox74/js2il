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
    If Type(Primitive(x)) is not String or Type(Primitive(y)) is not String,
    then operator x > y returns ToNumber(x) > ToNumber(y)
es5id: 11.8.2_A3.1_T2.1
description: >
    Type(Primitive(x)) is different from Type(Primitive(y)) and both
    types vary between Number (primitive or object) and Boolean
    (primitive and object)
---*/

//CHECK#1
if (true > 1 !== false) {
  throw new Test262Error('#1: true > 1 === false');
}

//CHECK#2
if (1 > true !== false) {
  throw new Test262Error('#2: 1 > true === false');
}

//CHECK#3
if (new Boolean(true) > 1 !== false) {
  throw new Test262Error('#3: new Boolean(true) > 1 === false');
}

//CHECK#4
if (1 > new Boolean(true) !== false) {
  throw new Test262Error('#4: 1 > new Boolean(true) === false');
}

//CHECK#5
if (true > new Number(1) !== false) {
  throw new Test262Error('#5: true > new Number(1) === false');
}

//CHECK#6
if (new Number(1) > true !== false) {
  throw new Test262Error('#6: new Number(1) > true === false');
}

//CHECK#7
if (new Boolean(true) > new Number(1) !== false) {
  throw new Test262Error('#7: new Boolean(true) > new Number(1) === false');
}

//CHECK#8
if (new Number(1) > new Boolean(true) !== false) {
  throw new Test262Error('#8: new Number(1) > new Boolean(true) === false');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
