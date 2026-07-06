// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = null ?? 1 ^ 42;
assert.sameValue(x, 43, 'null ?? 1 ^ 42');

x = undefined ?? 1 ^ 42;
assert.sameValue(x, 43, 'null ?? 1 ^ 42');

x = false ?? 1 ^ 42;
assert.sameValue(x, false, 'false ?? 1 ^ 42');

x = true ?? 1 ^ 42;
assert.sameValue(x, true, 'true ?? 1 ^ 42');
