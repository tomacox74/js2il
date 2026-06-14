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
esid: sec-map.prototype.foreach
description: >
  Repeats for each non-empty record, in original key insertion order.
info: |
  Map.prototype.forEach ( callbackfn [ , thisArg ] )

  ...
  5. If thisArg was supplied, let T be thisArg; else let T be undefined.
  6. Let entries be the List that is the value of M’s [[MapData]] internal slot.
  7. Repeat for each Record {[[key]], [[value]]} e that is an element of
  entries, in original key insertion order
    a. If e.[[key]] is not empty, then
      i. Let funcResult be Call(callbackfn, T, «e.[[value]], e.[[key]], M»).
  ...
---*/

var map = new Map([
  ['foo', 'valid foo'],
  ['bar', false],
  ['baz', 'valid baz']
]);
map.set(0, false);
map.set(1, false);
map.set(2, 'valid 2');
map.delete(1);
map.delete('bar');

// Not setting a new key, just changing the value
map.set(0, 'valid 0');

var results = [];
var callback = function(value) {
  results.push(value);
};

map.forEach(callback);

assert.sameValue(results[0], 'valid foo');
assert.sameValue(results[1], 'valid baz');
assert.sameValue(results[2], 'valid 0');
assert.sameValue(results[3], 'valid 2');
assert.sameValue(results.length, 4);

map.clear();
results = [];

map.forEach(callback);
assert.sameValue(results.length, 0);
