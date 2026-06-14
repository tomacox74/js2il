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
info: Return the number value for the MV of Result(4)
esid: sec-parsefloat-string
description: Checking DecimalDigits . DecimalDigits_opt ExponentPart_opt
---*/

//CHECK#1
if (parseFloat("-11.") !== -11) {
  throw new Test262Error('#1: parseFloat("-11.") === -11. Actual: ' + (parseFloat("-11.")));
}

//CHECK#2
if (parseFloat("01.") !== 1) {
  throw new Test262Error('#2: parseFloat("01.") === 1. Actual: ' + (parseFloat("01.")));
}

//CHECK#3
if (parseFloat("+11.1") !== 11.1) {
  throw new Test262Error('#3: parseFloat("+11.1") === 11.1. Actual: ' + (parseFloat("+11.1")));
}

//CHECK#4
if (parseFloat("01.1") !== 1.1) {
  throw new Test262Error('#4: parseFloat("01.1") === 1.1. Actual: ' + (parseFloat("01.1")));
}

//CHECK#5
if (parseFloat("-11.e-1") !== -1.1) {
  throw new Test262Error('#5: parseFloat("-11.e-1") === -1.1. Actual: ' + (parseFloat("-11.e-1")));
}

//CHECK#6
if (parseFloat("01.e1") !== 10) {
  throw new Test262Error('#6: parseFloat("01.e1") === 10. Actual: ' + (parseFloat("01.e1")));
}

//CHECK#7
if (parseFloat("+11.22e-1") !== 1.122) {
  throw new Test262Error('#7: parseFloat("+11.22e-1") === 1.122. Actual: ' + (parseFloat("+11.22e-1")));
}

//CHECK#8
if (parseFloat("01.01e1") !== 10.1) {
  throw new Test262Error('#8: parseFloat("01.01e1") === 10.1. Actual: ' + (parseFloat("01.01e1")));
}

//CHECK#9
if (parseFloat("001.") !== 1) {
  throw new Test262Error('#9: parseFloat("001.") === 1. Actual: ' + (parseFloat("001.")));
}

//CHECK#10
if (parseFloat("010.") !== 10) {
  throw new Test262Error('#10: parseFloat("010.") === 10. Actual: ' + (parseFloat("010.")));
}


console.log(true);
