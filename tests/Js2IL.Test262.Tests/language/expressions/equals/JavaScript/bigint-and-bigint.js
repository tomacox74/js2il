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

assert.sameValue(0n == 0n, true, 'The result of (0n == 0n) is true');
assert.sameValue(1n == 1n, true, 'The result of (1n == 1n) is true');
assert.sameValue(-1n == -1n, true, 'The result of (-1n == -1n) is true');
assert.sameValue(0n == -0n, true, 'The result of (0n == -0n) is true');
assert.sameValue(-0n == 0n, true, 'The result of (-0n == 0n) is true');
assert.sameValue(0n == 1n, false, 'The result of (0n == 1n) is false');
assert.sameValue(1n == 0n, false, 'The result of (1n == 0n) is false');
assert.sameValue(0n == -1n, false, 'The result of (0n == -1n) is false');
assert.sameValue(-1n == 0n, false, 'The result of (-1n == 0n) is false');
assert.sameValue(1n == -1n, false, 'The result of (1n == -1n) is false');
assert.sameValue(-1n == 1n, false, 'The result of (-1n == 1n) is false');

assert.sameValue(
  0x1fffffffffffff01n == 0x1fffffffffffff01n,
  true,
  'The result of (0x1fffffffffffff01n == 0x1fffffffffffff01n) is true'
);

assert.sameValue(
  0x1fffffffffffff01n == 0x1fffffffffffff02n,
  false,
  'The result of (0x1fffffffffffff01n == 0x1fffffffffffff02n) is false'
);

assert.sameValue(
  0x1fffffffffffff02n == 0x1fffffffffffff01n,
  false,
  'The result of (0x1fffffffffffff02n == 0x1fffffffffffff01n) is false'
);

assert.sameValue(
  -0x1fffffffffffff01n == -0x1fffffffffffff01n,
  true,
  'The result of (-0x1fffffffffffff01n == -0x1fffffffffffff01n) is true'
);

assert.sameValue(
  -0x1fffffffffffff01n == -0x1fffffffffffff02n,
  false,
  'The result of (-0x1fffffffffffff01n == -0x1fffffffffffff02n) is false'
);

assert.sameValue(
  -0x1fffffffffffff02n == -0x1fffffffffffff01n,
  false,
  'The result of (-0x1fffffffffffff02n == -0x1fffffffffffff01n) is false'
);

assert.sameValue(
  0x10000000000000000n == 0n,
  false,
  'The result of (0x10000000000000000n == 0n) is false'
);

assert.sameValue(
  0n == 0x10000000000000000n,
  false,
  'The result of (0n == 0x10000000000000000n) is false'
);

assert.sameValue(
  0x10000000000000000n == 1n,
  false,
  'The result of (0x10000000000000000n == 1n) is false'
);

assert.sameValue(
  1n == 0x10000000000000000n,
  false,
  'The result of (1n == 0x10000000000000000n) is false'
);

assert.sameValue(
  0x10000000000000000n == -1n,
  false,
  'The result of (0x10000000000000000n == -1n) is false'
);

assert.sameValue(
  -1n == 0x10000000000000000n,
  false,
  'The result of (-1n == 0x10000000000000000n) is false'
);

assert.sameValue(
  0x10000000000000001n == 0n,
  false,
  'The result of (0x10000000000000001n == 0n) is false'
);

assert.sameValue(
  0n == 0x10000000000000001n,
  false,
  'The result of (0n == 0x10000000000000001n) is false'
);

assert.sameValue(
  -0x10000000000000000n == 0n,
  false,
  'The result of (-0x10000000000000000n == 0n) is false'
);

assert.sameValue(
  0n == -0x10000000000000000n,
  false,
  'The result of (0n == -0x10000000000000000n) is false'
);

assert.sameValue(
  -0x10000000000000000n == 1n,
  false,
  'The result of (-0x10000000000000000n == 1n) is false'
);

assert.sameValue(
  1n == -0x10000000000000000n,
  false,
  'The result of (1n == -0x10000000000000000n) is false'
);

assert.sameValue(
  -0x10000000000000000n == -1n,
  false,
  'The result of (-0x10000000000000000n == -1n) is false'
);

assert.sameValue(
  -1n == -0x10000000000000000n,
  false,
  'The result of (-1n == -0x10000000000000000n) is false'
);

assert.sameValue(
  -0x10000000000000001n == 0n,
  false,
  'The result of (-0x10000000000000001n == 0n) is false'
);

assert.sameValue(
  0n == -0x10000000000000001n,
  false,
  'The result of (0n == -0x10000000000000001n) is false'
);

assert.sameValue(
  0x10000000000000000n == 0x100000000n,
  false,
  'The result of (0x10000000000000000n == 0x100000000n) is false'
);

assert.sameValue(
  0x100000000n == 0x10000000000000000n,
  false,
  'The result of (0x100000000n == 0x10000000000000000n) is false'
);
