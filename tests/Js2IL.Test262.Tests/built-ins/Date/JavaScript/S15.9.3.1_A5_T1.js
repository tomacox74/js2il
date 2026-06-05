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
info: |
    The [[Value]] property of the newly constructed object
    is set by following steps:
    8. If Result(1) is not NaN and 0 <= ToInteger(Result(1)) <= 99, Result(8) is
    1900+ToInteger(Result(1)); otherwise, Result(8) is Result(1)
    9. Compute MakeDay(Result(8), Result(2), Result(3))
    10. Compute MakeTime(Result(4), Result(5), Result(6), Result(7))
    11. Compute MakeDate(Result(9), Result(10))
    12. Set the [[Value]] property of the newly constructed object to
    TimeClip(UTC(Result(11)))
esid: sec-date-year-month-date-hours-minutes-seconds-ms
description: 2 arguments, (year, month)
includes: [assertRelativeDateMs.js]
---*/

assertRelativeDateMs(new Date(1899, 11), -2211667200000);

assertRelativeDateMs(new Date(1899, 12), -2208988800000);

assertRelativeDateMs(new Date(1900, 0), -2208988800000);

assertRelativeDateMs(new Date(1969, 11), -2678400000);

assertRelativeDateMs(new Date(1969, 12), 0);

assertRelativeDateMs(new Date(1970, 0), 0);

assertRelativeDateMs(new Date(1999, 11), 944006400000);

assertRelativeDateMs(new Date(1999, 12), 946684800000);

assertRelativeDateMs(new Date(2000, 0), 946684800000);

assertRelativeDateMs(new Date(2099, 11), 4099766400000);

assertRelativeDateMs(new Date(2099, 12), 4102444800000);

assertRelativeDateMs(new Date(2100, 0), 4102444800000);
