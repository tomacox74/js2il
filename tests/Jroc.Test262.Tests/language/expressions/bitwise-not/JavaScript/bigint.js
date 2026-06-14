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

// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: Bitwise NOT for BigInt values
esid: sec-numeric-types-bigint-bitwiseNOT
info: |
  BigInt::bitwiseNOT (x)

  The abstract operation BigInt::bitwiseNOT with an argument x of BigInt type returns the one's complement of x; that is, -x - 1.

features: [BigInt]
---*/

assert.sameValue(~0n, -1n, "~0n === -1n");
assert.sameValue(~(0n), -1n, "~(0n) === -1n");
assert.sameValue(~1n, -2n, "~1n === -2n");
assert.sameValue(~-1n, 0n, "~-1n === 0n");
assert.sameValue(~(-1n), 0n, "~(-1n) === 0n");
assert.sameValue(~~1n, 1n, "~~1n === 1n");
assert.sameValue(~0x5an, -0x5bn, "~0x5an === -0x5bn");
assert.sameValue(~-0x5an, 0x59n, "~-0x5an === 0x59n");
assert.sameValue(~0xffn, -0x100n, "~0xffn === -0x100n");
assert.sameValue(~-0xffn, 0xfen, "~-0xffn === 0xfen");
assert.sameValue(~0xffffn, -0x10000n, "~0xffffn === -0x10000n");
assert.sameValue(~-0xffffn, 0xfffen, "~-0xffffn === 0xfffen");
assert.sameValue(~0xffffffffn, -0x100000000n, "~0xffffffffn === -0x100000000n");
assert.sameValue(~-0xffffffffn, 0xfffffffen, "~-0xffffffffn === 0xfffffffen");
assert.sameValue(
  ~0xffffffffffffffffn, -0x10000000000000000n,
  "~0xffffffffffffffffn === -0x10000000000000000n");
assert.sameValue(
  ~-0xffffffffffffffffn, 0xfffffffffffffffen,
  "~-0xffffffffffffffffn === 0xfffffffffffffffen");
assert.sameValue(
  ~0x123456789abcdef0fedcba9876543210n, -0x123456789abcdef0fedcba9876543211n,
  "~0x123456789abcdef0fedcba9876543210n === -0x123456789abcdef0fedcba9876543211n");
