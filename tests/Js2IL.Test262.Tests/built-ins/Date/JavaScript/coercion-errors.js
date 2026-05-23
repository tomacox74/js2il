// Copyright (C) 2017 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-date-year-month-date-hours-minutes-seconds-ms
description: Abrupt completions from coercing input values
info: |
  3. If NewTarget is not undefined, then
    a. Let y be ? ToNumber(year).
    b. Let m be ? ToNumber(month).
    c. If date is supplied, let dt be ? ToNumber(date); else let dt be 1.
    d. If hours is supplied, let h be ? ToNumber(hours); else let h be 0.
    e. If minutes is supplied, let min be ? ToNumber(minutes); else let min be 0.
    f. If seconds is supplied, let s be ? ToNumber(seconds); else let s be 0.
    g. If ms is supplied, let milli be ? ToNumber(ms); else let milli be 0.
    h. If y is not NaN and 0 ≤ ToInteger(y) ≤ 99, let yr be 1900+ToInteger(y); otherwise,
      let yr be y.
    i. Let finalDate be MakeDate(MakeDay(yr, m, dt), MakeTime(h, min, s, milli)).
    j. Let O be ? OrdinaryCreateFromConstructor(NewTarget, "%DatePrototype%", « [[DateValue]] »).
    k. Set O.[[DateValue]] to TimeClip(UTC(finalDate)).
    l. Return O.
---*/

function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
    throw new Test262Error(message);
}

function $DONE(error) {
    if (error) {
        throw error;
    }
}

function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

function __test262FormatMessage(message, fallback) {
    return message || fallback || 'Assertion failed';
}

function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message));
    }
}

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, 'Expected SameValue'));
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, 'Expected values to differ'));
    }
};

assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;

assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try {
        fn();
    } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, 'Expected function to throw'));
    }
};

function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) {
        return false;
    }

    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) {
            return false;
        }
    }

    return true;
}

assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, 'Expected arrays to match'));
    }
};

function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
        passed = __test262SameValue(actual.value, desc.value);
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
        passed = actual.writable === desc.writable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
        passed = actual.enumerable === desc.enumerable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
        passed = actual.configurable === desc.configurable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
        passed = actual.get === desc.get;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
        passed = actual.set === desc.set;
    }

    console.log(passed);
    if (!passed) {
        throw new Error('verifyProperty failed for ' + name);
    }
}

var thrower = { toString: function() { throw new Test262Error(); } };
var counter = { toString: function() { callCount += 1; } };
var callCount = 0;

assert.throws(Test262Error, function() {
  new Date(thrower, counter);
}, 'year');
assert.sameValue(callCount, 0, 'coercion halts following error from "year"');

assert.throws(Test262Error, function() {
  new Date(0, thrower, counter);
}, 'month');
assert.sameValue(callCount, 0, 'coercion halts following error from "month"');

assert.throws(Test262Error, function() {
  new Date(0, 0, thrower, counter);
}, 'date');
assert.sameValue(callCount, 0, 'coercion halts following error from "date"');

assert.throws(Test262Error, function() {
  new Date(0, 0, 1, thrower, counter);
}, 'hours');
assert.sameValue(callCount, 0, 'coercion halts following error from "hours"');

assert.throws(Test262Error, function() {
  new Date(0, 0, 1, 0, thrower, counter);
}, 'minutes');
assert.sameValue(
  callCount, 0, 'coercion halts following error from "minutes"'
);

assert.throws(Test262Error, function() {
  new Date(0, 0, 1, 0, 0, thrower, counter);
}, 'seconds');
assert.sameValue(
  callCount, 0, 'coercion halts following error from "seconds"'
);

assert.throws(Test262Error, function() {
  new Date(0, 0, 1, 0, 0, 0, thrower);
}, 'ms');
