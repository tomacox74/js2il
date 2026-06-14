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
    1. Call ToNumber(year)
    2. Call ToNumber(month)
    3. If date is supplied use ToNumber(date)
    4. If hours is supplied use ToNumber(hours)
    5. If minutes is supplied use ToNumber(minutes)
    6. If seconds is supplied use ToNumber(seconds)
    7. If ms is supplied use ToNumber(ms)
esid: sec-date-year-month-date-hours-minutes-seconds-ms
description: 7 arguments, (year, month, date, hours, minutes, seconds, ms)
---*/

function PoisonedValueOf(val) {
  this.value = val;
  this.valueOf = function() {
    throw new Test262Error();
  };
  this.toString = function() {};
}

assert.throws(Test262Error, () => {
  new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(new PoisonedValueOf(1), new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(1, new PoisonedValueOf(2), new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(1, 2, new PoisonedValueOf(3), new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(1, 2, 3, new PoisonedValueOf(4), new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, 4, new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(1, 2, 3, 4, new PoisonedValueOf(5), new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, 4, 5, new PoisonedValueOf(6), new PoisonedValueOf(7));
}, '`new Date(1, 2, 3, 4, 5, new PoisonedValueOf(6), new PoisonedValueOf(7))` throws a Test262Error exception');

assert.throws(Test262Error, () => {
  new Date(1, 2, 3, 4, 5, 6, new PoisonedValueOf(7));
}, '`new Date(1, 2, 3, 4, 5, 6, new PoisonedValueOf(7))` throws a Test262Error exception');
