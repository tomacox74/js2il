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

assert.sameValue(0n === '', false, 'The result of (0n === "") is false');
assert.sameValue('' === 0n, false, 'The result of ("" === 0n) is false');
assert.sameValue(0n === '-0', false, 'The result of (0n === "-0") is false');
assert.sameValue('-0' === 0n, false, 'The result of ("-0" === 0n) is false');
assert.sameValue(0n === '0', false, 'The result of (0n === "0") is false');
assert.sameValue('0' === 0n, false, 'The result of ("0" === 0n) is false');
assert.sameValue(0n === '-1', false, 'The result of (0n === "-1") is false');
assert.sameValue('-1' === 0n, false, 'The result of ("-1" === 0n) is false');
assert.sameValue(0n === '1', false, 'The result of (0n === "1") is false');
assert.sameValue('1' === 0n, false, 'The result of ("1" === 0n) is false');
assert.sameValue(0n === 'foo', false, 'The result of (0n === "foo") is false');
assert.sameValue('foo' === 0n, false, 'The result of ("foo" === 0n) is false');
assert.sameValue(1n === '', false, 'The result of (1n === "") is false');
assert.sameValue('' === 1n, false, 'The result of ("" === 1n) is false');
assert.sameValue(1n === '-0', false, 'The result of (1n === "-0") is false');
assert.sameValue('-0' === 1n, false, 'The result of ("-0" === 1n) is false');
assert.sameValue(1n === '0', false, 'The result of (1n === "0") is false');
assert.sameValue('0' === 1n, false, 'The result of ("0" === 1n) is false');
assert.sameValue(1n === '-1', false, 'The result of (1n === "-1") is false');
assert.sameValue('-1' === 1n, false, 'The result of ("-1" === 1n) is false');
assert.sameValue(1n === '1', false, 'The result of (1n === "1") is false');
assert.sameValue('1' === 1n, false, 'The result of ("1" === 1n) is false');
assert.sameValue(1n === 'foo', false, 'The result of (1n === "foo") is false');
assert.sameValue('foo' === 1n, false, 'The result of ("foo" === 1n) is false');
assert.sameValue(-1n === '-', false, 'The result of (-1n === "-") is false');
assert.sameValue('-' === -1n, false, 'The result of ("-" === -1n) is false');
assert.sameValue(-1n === '-0', false, 'The result of (-1n === "-0") is false');
assert.sameValue('-0' === -1n, false, 'The result of ("-0" === -1n) is false');
assert.sameValue(-1n === '-1', false, 'The result of (-1n === "-1") is false');
assert.sameValue('-1' === -1n, false, 'The result of ("-1" === -1n) is false');
assert.sameValue(-1n === '-foo', false, 'The result of (-1n === "-foo") is false');
assert.sameValue('-foo' === -1n, false, 'The result of ("-foo" === -1n) is false');

assert.sameValue(
  900719925474099101n === '900719925474099101',
  false,
  'The result of (900719925474099101n === "900719925474099101") is false'
);

assert.sameValue(
  '900719925474099101' === 900719925474099101n,
  false,
  'The result of ("900719925474099101" === 900719925474099101n) is false'
);

assert.sameValue(
  900719925474099102n === '900719925474099101',
  false,
  'The result of (900719925474099102n === "900719925474099101") is false'
);

assert.sameValue(
  '900719925474099101' === 900719925474099102n,
  false,
  'The result of ("900719925474099101" === 900719925474099102n) is false'
);
