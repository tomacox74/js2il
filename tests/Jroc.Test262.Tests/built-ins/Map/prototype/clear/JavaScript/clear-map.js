function Test262Error(message) {
    this.message = message || "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
Test262Error.prototype.name = "Test262Error";

function __test262SameValue(actual, expected) {
    if (actual === expected) {
        return actual !== 0 || 1 / actual === 1 / expected;
    }
    return actual !== actual && expected !== expected;
}

function __test262FormatMessage(message, fallback) {
    return message || fallback;
}

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Assertion failed"));
    }
};

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected SameValue"));
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected values to differ"));
    }
};

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
        throw new Error(__test262FormatMessage(message, "Expected function to throw"));
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
        throw new Error(__test262FormatMessage(message, "Expected arrays to match"));
    }
};

// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-map.prototype.clear
description: >
  Clears a Map.
info: |
  Map.prototype.clear ( )

  ...
  4. Let entries be the List that is the value of M’s [[MapData]] internal slot.
  5. Repeat for each Record {[[key]], [[value]]} p that is an element of
  entries,
    a. Set p.[[key]] to empty.
    b. Set p.[[value]] to empty.
  6. Return undefined.
features: [Symbol]
---*/

var m1 = new Map([
  ['foo', 'bar'],
  [1, 1]
]);
var m2 = new Map();
var m3 = new Map();
m2.set('foo', 'bar');
m2.set(1, 1);
m2.set(Symbol('a'), Symbol('a'));

m1.clear();
m2.clear();
m3.clear();

assert.sameValue(m1.size, 0);
assert.sameValue(m2.size, 0);
assert.sameValue(m3.size, 0);
