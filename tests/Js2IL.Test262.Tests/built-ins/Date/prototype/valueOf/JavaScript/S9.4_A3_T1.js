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
function assertRelativeDateMs(date, expectedMs) {
    var passed = Object.is(date.valueOf(), expectedMs);
    console.log(passed);
    if (!passed) { throw new Error('Expected date value ' + expectedMs); }
}
function isConstructor(fn) {
    try { Reflect.construct(Object, [], fn); return true; } catch (error) { return false; }
}

// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-date.prototype.valueof
info: |
    Result of ToInteger(value) conversion is the result of computing
    sign(ToNumber(value)) * floor(abs(ToNumber(value)))
es5id: 9.4_A3_T1
description: For testing constructor Date(Number) is used
---*/

// CHECK#1
var d1 = new Date(6.54321);
assert.sameValue(d1.valueOf(), 6, 'd1.valueOf() must return 6');

// CHECK#2
var d2 = new Date(-6.54321);
assert.sameValue(d2.valueOf(), -6, 'd2.valueOf() must return -6');

// CHECK#3
var d3 = new Date(6.54321e2);
assert.sameValue(d3.valueOf(), 654, 'd3.valueOf() must return 654');

// CHECK#4
var d4 = new Date(-6.54321e2);
assert.sameValue(d4.valueOf(), -654, 'd4.valueOf() must return -654');

// CHECK#5
var d5 = new Date(0.654321e1);
assert.sameValue(d5.valueOf(), 6, 'd5.valueOf() must return 6');

// CHECK#6
var d6 = new Date(-0.654321e1);
assert.sameValue(d6.valueOf(), -6, 'd6.valueOf() must return -6');

// CHECK#7
var d7 = new Date(true);
assert.sameValue(d7.valueOf(), 1, 'd7.valueOf() must return 1');

// CHECK#8
var d8 = new Date(false);
assert.sameValue(d8.valueOf(), 0, 'd8.valueOf() must return 0');

// CHECK#9
var d9 = new Date(1.23e15);
assert.sameValue(d9.valueOf(), 1.23e15, 'd9.valueOf() must return 1.23e15');

// CHECK#10
var d10 = new Date(-1.23e15);
assert.sameValue(d10.valueOf(), -1.23e15, 'd10.valueOf() must return -1.23e15');

// CHECK#11
var d11 = new Date(1.23e-15);
assert.sameValue(d11.valueOf(), 0, 'd11.valueOf() must return 0');

// CHECK#12
var d12 = new Date(-1.23e-15);
assert.sameValue(d12.valueOf(), 0, 'd12.valueOf() must return 0');
