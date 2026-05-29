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
esid: sec-map-iterable
description: >
  Throws a TypeError if iterable items are not Objects.
info: |
  Map ( [ iterable ] )

  ...
  9. Repeat
    ...
    d. Let nextItem be IteratorValue(next).
    e. ReturnIfAbrupt(nextItem).
    f. If Type(nextItem) is not Object,
      i. Let error be Completion{[[type]]: throw, [[value]]: a newly created
      TypeError object, [[target]]:empty}.
      ii. Return IteratorClose(iter, error).
features: [Symbol]
---*/

assert.throws(TypeError, function() {
  new Map([1]);
});

assert.throws(TypeError, function() {
  new Map(['']);
});

assert.throws(TypeError, function() {
  new Map([true]);
});

assert.throws(TypeError, function() {
  new Map([null]);
});

assert.throws(TypeError, function() {
  new Map([Symbol('a')]);
});

assert.throws(TypeError, function() {
  new Map([undefined]);
});

assert.throws(TypeError, function() {
  new Map([
    ['a', 1],
    2
  ]);
});
