// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = null ?? undefined ?? 42;
assert.sameValue(x, 42, 'null ?? undefined ?? 42');

x = undefined ?? null ?? 42;
assert.sameValue(x, 42, 'undefined ?? null ?? 42');

x = null ?? null ?? 42;
assert.sameValue(x, 42, 'null ?? null ?? 42');

x = undefined ?? undefined ?? 42;
assert.sameValue(x, 42, 'null ?? null ?? 42');
