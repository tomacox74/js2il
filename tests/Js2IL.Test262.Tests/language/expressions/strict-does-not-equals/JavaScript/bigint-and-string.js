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
description: Strict inequality comparison of BigInt and String values
esid: sec-strict-equality-comparison
info: |
  1. If Type(x) is different from Type(y), return false.

features: [BigInt]
---*/
assert.sameValue(0n !== '', true, 'The result of (0n !== "") is true');
assert.sameValue('' !== 0n, true, 'The result of ("" !== 0n) is true');
assert.sameValue(0n !== '-0', true, 'The result of (0n !== "-0") is true');
assert.sameValue('-0' !== 0n, true, 'The result of ("-0" !== 0n) is true');
assert.sameValue(0n !== '0', true, 'The result of (0n !== "0") is true');
assert.sameValue('0' !== 0n, true, 'The result of ("0" !== 0n) is true');
assert.sameValue(0n !== '-1', true, 'The result of (0n !== "-1") is true');
assert.sameValue('-1' !== 0n, true, 'The result of ("-1" !== 0n) is true');
assert.sameValue(0n !== '1', true, 'The result of (0n !== "1") is true');
assert.sameValue('1' !== 0n, true, 'The result of ("1" !== 0n) is true');
assert.sameValue(0n !== 'foo', true, 'The result of (0n !== "foo") is true');
assert.sameValue('foo' !== 0n, true, 'The result of ("foo" !== 0n) is true');
assert.sameValue(1n !== '', true, 'The result of (1n !== "") is true');
assert.sameValue('' !== 1n, true, 'The result of ("" !== 1n) is true');
assert.sameValue(1n !== '-0', true, 'The result of (1n !== "-0") is true');
assert.sameValue('-0' !== 1n, true, 'The result of ("-0" !== 1n) is true');
assert.sameValue(1n !== '0', true, 'The result of (1n !== "0") is true');
assert.sameValue('0' !== 1n, true, 'The result of ("0" !== 1n) is true');
assert.sameValue(1n !== '-1', true, 'The result of (1n !== "-1") is true');
assert.sameValue('-1' !== 1n, true, 'The result of ("-1" !== 1n) is true');
assert.sameValue(1n !== '1', true, 'The result of (1n !== "1") is true');
assert.sameValue('1' !== 1n, true, 'The result of ("1" !== 1n) is true');
assert.sameValue(1n !== 'foo', true, 'The result of (1n !== "foo") is true');
assert.sameValue('foo' !== 1n, true, 'The result of ("foo" !== 1n) is true');
assert.sameValue(-1n !== '-', true, 'The result of (-1n !== "-") is true');
assert.sameValue('-' !== -1n, true, 'The result of ("-" !== -1n) is true');
assert.sameValue(-1n !== '-0', true, 'The result of (-1n !== "-0") is true');
assert.sameValue('-0' !== -1n, true, 'The result of ("-0" !== -1n) is true');
assert.sameValue(-1n !== '-1', true, 'The result of (-1n !== "-1") is true');
assert.sameValue('-1' !== -1n, true, 'The result of ("-1" !== -1n) is true');
assert.sameValue(-1n !== '-foo', true, 'The result of (-1n !== "-foo") is true');
assert.sameValue('-foo' !== -1n, true, 'The result of ("-foo" !== -1n) is true');

assert.sameValue(
  900719925474099101n !== '900719925474099101',
  true,
  'The result of (900719925474099101n !== "900719925474099101") is true'
);

assert.sameValue(
  '900719925474099101' !== 900719925474099101n,
  true,
  'The result of ("900719925474099101" !== 900719925474099101n) is true'
);

assert.sameValue(
  900719925474099102n !== '900719925474099101',
  true,
  'The result of (900719925474099102n !== "900719925474099101") is true'
);

assert.sameValue(
  '900719925474099101' !== 900719925474099102n,
  true,
  'The result of ("900719925474099101" !== 900719925474099102n) is true'
);
