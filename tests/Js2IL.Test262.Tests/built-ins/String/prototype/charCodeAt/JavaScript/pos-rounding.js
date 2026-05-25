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

// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-string.prototype.charcodeat
description: Rounding of the provided "pos" number
info: |
  [...]
  3. Let position be ? ToInteger(pos).
  [...]

  7.1.4 ToInteger

  1. Let number be ? ToNumber(argument).
  2. If number is NaN, return +0.
  3. If number is +0, -0, +∞, or -∞, return number.
  4. Return the number value that is the same sign as number and whose
     magnitude is floor(abs(number)). 
---*/

var aCode = 97;
var bCode = 98;

assert.sameValue('abc'.charCodeAt(-0.99999), aCode, '-0.99999');
assert.sameValue('abc'.charCodeAt(-0.00001), aCode, '-0.00001');
assert.sameValue('abc'.charCodeAt(0.00001), aCode, '0.00001');
assert.sameValue('abc'.charCodeAt(0.99999), aCode, '0.99999');
assert.sameValue('abc'.charCodeAt(1.00001), bCode, '1.00001');
assert.sameValue('abc'.charCodeAt(1.99999), bCode, '1.99999');
