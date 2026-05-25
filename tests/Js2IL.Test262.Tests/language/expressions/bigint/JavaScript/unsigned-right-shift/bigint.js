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

// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: Unsigned right shift always throws for BigInt values
esid: sec-numeric-types-bigint-unsignedRightShift
info: |
  BigInt::unsignedRightShift (x, y)

  The abstract operation BigInt::unsignedRightShift with two arguments x and y of type BigInt:

  1. Throw a TypeError exception.

features: [BigInt]
---*/

assert.throws(TypeError, function() { 0n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> 1n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> 2n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> 3n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> -1n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> -2n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 5n >>> -3n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 0n >>> 128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 0n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 582n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 582n >>> 127n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 582n >>> 128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 582n >>> 129n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 582n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> 64n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> 32n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> 16n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -16n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -32n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -64n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -127n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { 405972677036361916727469983882855107238581880n >>> -129n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> 1n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> 2n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> 3n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> -1n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> -2n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -5n >>> -3n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -1n >>> 128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -1n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -1n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -582n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -582n >>> 127n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -582n >>> 128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -582n >>> 129n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -582n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> 64n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> 32n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> 16n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> 0n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -16n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -32n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -64n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -127n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -128n; }, "bigint >>> bigint throws a TypeError");
assert.throws(TypeError, function() { -405972677036361916727469983882855107238581880n >>> -129n; }, "bigint >>> bigint throws a TypeError");
