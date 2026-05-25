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
info: If Type(x) is different from Type(y), return false
es5id: 11.9.4_A8_T4
description: x or y is null or undefined
---*/

//CHECK#1
if (undefined === null) {
  throw new Test262Error('#1: undefined !== null');
}

//CHECK#2
if (null === undefined) {
  throw new Test262Error('#2: null !== undefined');
}

//CHECK#3
if (null === 0) {
  throw new Test262Error('#3: null !== 0');
}

//CHECK#4
if (0 === null) {
  throw new Test262Error('#4: 0 !== null');
}

//CHECK#5
if (null === false) {
  throw new Test262Error('#5: null !== false');
}

//CHECK#6
if (false === null) {
  throw new Test262Error('#6: false !== null');
}

//CHECK#7
if (undefined === false) {
  throw new Test262Error('#7: undefined !== false');
}

//CHECK#8
if (false === undefined) {
  throw new Test262Error('#8: false !== undefined');
}

//CHECK#9
if (null === new Object()) {
  throw new Test262Error('#9: null !== new Object()');
}

//CHECK#10
if (new Object() === null) {
  throw new Test262Error('#10: new Object() !== null');
}

//CHECK#11
if (null === "null") {
  throw new Test262Error('#11: null !== "null"');
}

//CHECK#12
if ("null" === null) {
  throw new Test262Error('#12: "null" !== null');
}

//CHECK#13
if (undefined === "undefined") {
  throw new Test262Error('#13: undefined !== "undefined"');
}

//CHECK#14
if ("undefined" === undefined) {
  throw new Test262Error('#14: "undefined" !== undefined');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
