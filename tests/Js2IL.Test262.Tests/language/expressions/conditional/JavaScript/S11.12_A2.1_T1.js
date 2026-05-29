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
info: "Operator x ? y : z uses GetValue"
es5id: 11.12_A2.1_T1
description: Either Type is not Reference or GetBase is not null
---*/

//CHECK#1
if ((true ? false : true) !== false) {
  throw new Test262Error('#1: (true ? false : true) === false');
}

//CHECK#2
if ((false ? false : true) !== true) {
  throw new Test262Error('#2: (false ? false : true) === true');
}

//CHECK#3
var x = new Boolean(true);
var y = new Boolean(false);
if ((x ? y : true) !== y) {
  throw new Test262Error('#3: var x = new Boolean(true); var y = new Boolean(false); (x ? y : true) === y');
}

//CHECK#4
var z = new Boolean(true);
if ((false ? false : z) !== z) {
  throw new Test262Error('#4: var z = new Boolean(true); (false ? false : z) === z');
}

//CHECK#5
var x = new Boolean(true);
var y = new Boolean(false);
var z = new Boolean(true);
if ((x ? y : z) !== y) {
  throw new Test262Error('#5: var x = new Boolean(true); var y = new Boolean(false); var z = new Boolean(true); (x ? y : z) === y');
}

//CHECK#6
var x = false;
var y = new Boolean(false);
var z = new Boolean(true);
if ((x ? y : z) !== z) {
  throw new Test262Error('#6: var x = false; var y = new Boolean(false); var z = new Boolean(true); (x ? y : z) === z');
}

    console.log(true);
} catch (error) {
    console.log(false);
}
