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
esid: sec-map.prototype.entries
description: >
  Returns an iterator.
info: |
  Map.prototype.entries ( )

  ...
  2. Return CreateMapIterator(M, "key+value").

  23.1.5.1 CreateMapIterator Abstract Operation

  ...
  7. Return iterator.
---*/

var map = new Map();
map.set('a',1);
map.set('b',2);
map.set('c',3);

var iterator = map.entries();
var result;

result = iterator.next();
assert.sameValue(result.value[0], 'a', 'First result `value` ("key")');
assert.sameValue(result.value[1], 1, 'First result `value` ("value")');
assert.sameValue(result.value.length, 2, 'First result `value` (length)');
assert.sameValue(result.done, false, 'First result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 'b', 'Second result `value` ("key")');
assert.sameValue(result.value[1], 2, 'Second result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Second result `value` (length)');
assert.sameValue(result.done, false, 'Second result `done` flag');

result = iterator.next();
assert.sameValue(result.value[0], 'c', 'Third result `value` ("key")');
assert.sameValue(result.value[1], 3, 'Third result `value` ("value")');
assert.sameValue(result.value.length, 2, 'Third result `value` (length)');
assert.sameValue(result.done, false, 'Third result `done` flag');

result = iterator.next();
assert.sameValue(result.value, undefined, 'Exhausted result `value`');
assert.sameValue(result.done, true, 'Exhausted result `done` flag');

result = iterator.next();
assert.sameValue(
  result.value, undefined, 'Exhausted result `value` (repeated request)'
);
assert.sameValue(
  result.done, true, 'Exhausted result `done` flag (repeated request)'
);
