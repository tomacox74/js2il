// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
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
