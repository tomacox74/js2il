// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 15.12.2-2-1
description: >
    JSON.parse - parsing an object where property name is a null
    character
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

var nullChars = new Array();
nullChars[0] = '\"\u0000\"';
nullChars[1] = '\"\u0001\"';
nullChars[2] = '\"\u0002\"';
nullChars[3] = '\"\u0003\"';
nullChars[4] = '\"\u0004\"';
nullChars[5] = '\"\u0005\"';
nullChars[6] = '\"\u0006\"';
nullChars[7] = '\"\u0007\"';
nullChars[8] = '\"\u0008\"';
nullChars[9] = '\"\u0009\"';
nullChars[10] = '\"\u000A\"';
nullChars[11] = '\"\u000B\"';
nullChars[12] = '\"\u000C\"';
nullChars[13] = '\"\u000D\"';
nullChars[14] = '\"\u000E\"';
nullChars[15] = '\"\u000F\"';
nullChars[16] = '\"\u0010\"';
nullChars[17] = '\"\u0011\"';
nullChars[18] = '\"\u0012\"';
nullChars[19] = '\"\u0013\"';
nullChars[20] = '\"\u0014\"';
nullChars[21] = '\"\u0015\"';
nullChars[22] = '\"\u0016\"';
nullChars[23] = '\"\u0017\"';
nullChars[24] = '\"\u0018\"';
nullChars[25] = '\"\u0019\"';
nullChars[26] = '\"\u001A\"';
nullChars[27] = '\"\u001B\"';
nullChars[28] = '\"\u001C\"';
nullChars[29] = '\"\u001D\"';
nullChars[30] = '\"\u001E\"';
nullChars[31] = '\"\u001F\"';

for (var index in nullChars) {
  assert.throws(SyntaxError, function() {
    var obj = JSON.parse('{ ' + nullChars[index] + ' : "John" } ');
  });
}
