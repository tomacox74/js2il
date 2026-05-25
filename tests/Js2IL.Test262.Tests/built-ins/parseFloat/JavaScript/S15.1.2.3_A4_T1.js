function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
function $ERROR(message) { throw new Test262Error(message); }
function $DONE(error) { if (error) { throw error; } }
function __test262SameValue(actual, expected) { return Object.is(actual, expected); }
function __test262FormatMessage(message, fallback) { return message || fallback || 'Assertion failed'; }
function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message)); }
}
assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected SameValue')); }
};
assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected values to differ')); }
};
assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;
assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try { fn(); } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected function to throw')); }
};
function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) { return false; }
    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) { return false; }
    }
    return true;
}
assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected arrays to match')); }
};
function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) { passed = __test262SameValue(actual.value, desc.value); }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) { passed = actual.writable === desc.writable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) { passed = actual.enumerable === desc.enumerable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) { passed = actual.configurable === desc.configurable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) { passed = actual.get === desc.get; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) { passed = actual.set === desc.set; }
    console.log(passed);
    if (!passed) { throw new Error('verifyProperty failed for ' + name); }
}

// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Compute the longest prefix of Result(2), which might be Result(2) itself,
    which satisfies the syntax of a StrDecimalLiteral
esid: sec-parsefloat-string
description: Some wrong number
---*/

//CHECK#1
if (parseFloat("0x") !== 0) {
  throw new Test262Error('#1: parseFloat("0x") === 0. Actual: ' + (parseFloat("0x")));
}

//CHECK#2
if (parseFloat("11x") !== 11) {
  throw new Test262Error('#2: parseFloat("11x") === 11. Actual: ' + (parseFloat("11x")));
}

//CHECK#3
if (parseFloat("11s1") !== 11) {
  throw new Test262Error('#3: parseFloat("11s1") === 11. Actual: ' + (parseFloat("11s1")));
}

//CHECK#4
if (parseFloat("11.s1") !== 11) {
  throw new Test262Error('#4: parseFloat("11.s1") === 11. Actual: ' + (parseFloat("11.s1")));
}

//CHECK#5
if (parseFloat(".0s1") !== 0) {
  throw new Test262Error('#5: parseFloat(".0s1") === 0. Actual: ' + (parseFloat(".0s1")));
}

//CHECK#6
if (parseFloat("1.s1") !== 1) {
  throw new Test262Error('#6: parseFloat("1.s1") === 1. Actual: ' + (parseFloat("1.s1")));
}

//CHECK#7
if (parseFloat("1..1") !== 1) {
  throw new Test262Error('#7: parseFloat("1..1") === 1. Actual: ' + (parseFloat("1..1")));
}

//CHECK#8
if (parseFloat("0.1.1") !== 0.1) {
  throw new Test262Error('#8: parseFloat("0.1.1") === 0.1. Actual: ' + (parseFloat("0.1.1")));
}

//CHECK#9
if (parseFloat("0. 1") !== 0) {
  throw new Test262Error('#9: parseFloat("0. 1") === 0. Actual: ' + (parseFloat("0. 1")));
}


console.log(true);
