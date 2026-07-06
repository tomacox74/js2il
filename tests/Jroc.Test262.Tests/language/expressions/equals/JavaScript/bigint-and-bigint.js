// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
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
