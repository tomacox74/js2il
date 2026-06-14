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
    If Type(Primitive(x)) is String or Type(Primitive(y)) is String, then
    operator x + y returns the result of concatenating ToString(x) followed
    by ToString(y)
es5id: 11.6.1_A3.2_T2.2
description: >
    Type(Primitive(x)) is different from Type(Primitive(y)) and both
    types vary between String (primitive or object) and Boolean
    (primitive and object)
---*/

//CHECK#1
if (true + "1" !== "true1") {
  throw new Test262Error('#1: true + "1" === "true1". Actual: ' + (true + "1"));
}

//CHECK#2
if ("1" + true !== "1true") {
  throw new Test262Error('#2: "1" + true === "1true". Actual: ' + ("1" + true));
}

//CHECK#3
if (new Boolean(true) + "1" !== "true1") {
  throw new Test262Error('#3: new Boolean(true) + "1" === "true1". Actual: ' + (new Boolean(true) + "1"));
}

//CHECK#4
if ("1" + new Boolean(true) !== "1true") {
  throw new Test262Error('#4: "1" + new Boolean(true) === "1true". Actual: ' + ("1" + new Boolean(true)));
}

//CHECK#5
if (true + new String("1") !== "true1") {
  throw new Test262Error('#5: true + new String("1") === "true1". Actual: ' + (true + new String("1")));
}

//CHECK#6
if (new String("1") + true !== "1true") {
  throw new Test262Error('#6: new String("1") + true === "1true". Actual: ' + (new String("1") + true));
}

//CHECK#7
if (new Boolean(true) + new String("1") !== "true1") {
  throw new Test262Error('#7: new Boolean(true) + new String("1") === "true1". Actual: ' + (new Boolean(true) + new String("1")));
}

//CHECK#8
if (new String("1") + new Boolean(true) !== "1true") {
  throw new Test262Error('#8: new String("1") + new Boolean(true) === "1true". Actual: ' + (new String("1") + new Boolean(true)));
}

    console.log(true);
} catch (error) {
    console.log(false);
}
