// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: String.prototype.indexOf type coercion for position parameter
esid: sec-string.prototype.indexof
info: |
  String.prototype.indexOf ( searchString [ , position ] )

  4. Let pos be ? ToInteger(position).
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

assert.sameValue("aaaa".indexOf("aa", 0), 0);
assert.sameValue("aaaa".indexOf("aa", 1), 1);
assert.sameValue("aaaa".indexOf("aa", -0.9), 0, "ToInteger: truncate towards 0");
assert.sameValue("aaaa".indexOf("aa", 0.9), 0, "ToInteger: truncate towards 0");
assert.sameValue("aaaa".indexOf("aa", 1.9), 1, "ToInteger: truncate towards 0");
assert.sameValue("aaaa".indexOf("aa", NaN), 0, "ToInteger: NaN => 0");
assert.sameValue("aaaa".indexOf("aa", Infinity), -1);
assert.sameValue("aaaa".indexOf("aa", undefined), 0, "ToInteger: undefined => NaN => 0");
assert.sameValue("aaaa".indexOf("aa", null), 0, "ToInteger: null => 0");
assert.sameValue("aaaa".indexOf("aa", false), 0, "ToInteger: false => 0");
assert.sameValue("aaaa".indexOf("aa", true), 1, "ToInteger: true => 1");
assert.sameValue("aaaa".indexOf("aa", "0"), 0, "ToInteger: parse Number");
assert.sameValue("aaaa".indexOf("aa", "1.9"), 1, "ToInteger: parse Number => 1.9 => 1");
assert.sameValue("aaaa".indexOf("aa", "Infinity"), -1, "ToInteger: parse Number");
assert.sameValue("aaaa".indexOf("aa", ""), 0, "ToInteger: unparseable string => NaN => 0");
assert.sameValue("aaaa".indexOf("aa", "foo"), 0, "ToInteger: unparseable string => NaN => 0");
assert.sameValue("aaaa".indexOf("aa", "true"), 0, "ToInteger: unparseable string => NaN => 0");
assert.sameValue("aaaa".indexOf("aa", 2), 2);
assert.sameValue("aaaa".indexOf("aa", "2"), 2, "ToInteger: parse Number");
assert.sameValue("aaaa".indexOf("aa", 2.9), 2, "ToInteger: truncate towards 0");
assert.sameValue("aaaa".indexOf("aa", "2.9"), 2, "ToInteger: parse Number => truncate towards 0");
assert.sameValue("aaaa".indexOf("aa", [0]), 0, 'ToInteger: [0].toString() => "0" => 0');
assert.sameValue("aaaa".indexOf("aa", ["1"]), 1, 'ToInteger: ["1"].toString() => "1" => 1');
assert.sameValue("aaaa".indexOf("aa", {}), 0,
  'ToInteger: ({}).toString() => "[object Object]" => NaN => 0');
assert.sameValue("aaaa".indexOf("aa", []), 0, 'ToInteger: [].toString() => "" => NaN => 0');
