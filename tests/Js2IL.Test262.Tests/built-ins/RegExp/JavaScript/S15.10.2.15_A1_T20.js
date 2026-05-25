function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) {
        throw new Error(message || 'Assertion failed');
    }
}

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || 'Expected SameValue');
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || 'Expected values to differ');
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
        throw new Error(message || 'Expected function to throw');
    }
};

// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The internal helper function CharacterRange takes two CharSet parameters A and B and performs the
    following:
    2. Let a be the one character in CharSet A.
    3. Let b be the one character in CharSet B.
    4. Let i be the character value of character a.
    5. Let j be the character value of character b.
    6. If i > j, throw a SyntaxError exception.
es5id: 15.10.2.15_A1_T20
description: >
    Checking if execution of "/[\u0061d-G]/.exec("a")" leads to
    throwing the correct exception
---*/

try {
  throw new Test262Error('#1.1: /[\\u0061d-G]/.exec("a") throw SyntaxError. Actual: ' + (new RegExp("[\\u0061d-G]").exec("a")));
} catch (e) {
  assert.sameValue(
    e instanceof SyntaxError,
    true,
    'The result of evaluating (e instanceof SyntaxError) is expected to be true'
  );
}

// TODO: Convert to assert.throws() format.
