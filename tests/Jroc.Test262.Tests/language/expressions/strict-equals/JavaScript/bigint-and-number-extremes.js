// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.sameValue = function(actual, expected) {
    console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected) {
    console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func) {
    try {
        func();
        console.log(false);
    } catch (error) {
        console.log(error instanceof expectedErrorConstructor || error.constructor === expectedErrorConstructor);
    }
};
assert.compareArray = function(actual, expected) {
    console.log(actual.length === expected.length && actual.every(function(value, index) {
        return Object.is(value, expected[index]);
    }));
};

assert.sameValue(
  1n === Number.MAX_VALUE,
  false,
  'The result of (1n === Number.MAX_VALUE) is false'
);

assert.sameValue(
  Number.MAX_VALUE === 1n,
  false,
  'The result of (Number.MAX_VALUE === 1n) is false'
);

assert.sameValue(
  1n === -Number.MAX_VALUE,
  false,
  'The result of (1n === -Number.MAX_VALUE) is false'
);

assert.sameValue(
  -Number.MAX_VALUE === 1n,
  false,
  'The result of (-Number.MAX_VALUE === 1n) is false'
);

assert.sameValue(
  0xfffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn === Number.MAX_VALUE,
  false,
  'The result of (0xfffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn === Number.MAX_VALUE) is false'
);

assert.sameValue(
  Number.MAX_VALUE === 0xfffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn,
  false,
  'The result of (Number.MAX_VALUE === 0xfffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn) is false'
);

assert.sameValue(
  0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000n === Number.MAX_VALUE,
  false,
  'The result of (0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000n === Number.MAX_VALUE) is false'
);

assert.sameValue(
  Number.MAX_VALUE === 0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000n,
  false,
  'The result of (Number.MAX_VALUE === 0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000n) is false'
);

assert.sameValue(
  0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001n === Number.MAX_VALUE,
  false,
  'The result of (0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001n === Number.MAX_VALUE) is false'
);

assert.sameValue(
  Number.MAX_VALUE === 0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001n,
  false,
  'The result of (Number.MAX_VALUE === 0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001n) is false'
);
