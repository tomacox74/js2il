// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = undefined;
x = false ?? 1;
assert.sameValue(x, false, 'false ?? 1');

x = undefined;
x = false ?? null;
assert.sameValue(x, false, 'false ?? null');

x = undefined;
x = false ?? undefined;
assert.sameValue(x, false, 'false ?? undefined');

x = undefined;
x = false ?? null ?? undefined;
assert.sameValue(x, false, 'false ?? null ?? undefined');

x = undefined;
x = false ?? undefined ?? null;
assert.sameValue(x, false, 'false ?? undefined ?? null');

x = undefined;
x = false ?? null ?? null;
assert.sameValue(x, false, 'false ?? null ?? null');

x = undefined;
x = false ?? undefined ?? undefined;
assert.sameValue(x, false, 'false ?? null ?? null');

x = undefined;
x = null ?? false ?? null;
assert.sameValue(x, false, 'null ?? false ?? null');

x = undefined;
x = null ?? false ?? undefined;
assert.sameValue(x, false, 'null ?? false ?? undefined');

x = undefined;
x = undefined ?? false ?? null;
assert.sameValue(x, false, 'undefined ?? false ?? null');

x = undefined;
x = undefined ?? false ?? undefined;
assert.sameValue(x, false, 'undefined ?? false ?? undefined');
