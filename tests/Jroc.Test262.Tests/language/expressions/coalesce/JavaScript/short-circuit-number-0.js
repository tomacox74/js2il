// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = undefined;
x = 0 ?? 1;
assert.sameValue(x, 0, '0 ?? 1');

x = undefined;
x = 0 ?? null;
assert.sameValue(x, 0, '0 ?? null');

x = undefined;
x = 0 ?? undefined;
assert.sameValue(x, 0, '0 ?? undefined');

x = undefined;
x = 0 ?? null ?? undefined;
assert.sameValue(x, 0, '0 ?? null ?? undefined');

x = undefined;
x = 0 ?? undefined ?? null;
assert.sameValue(x, 0, '0 ?? undefined ?? null');

x = undefined;
x = 0 ?? null ?? null;
assert.sameValue(x, 0, '0 ?? null ?? null');

x = undefined;
x = 0 ?? undefined ?? undefined;
assert.sameValue(x, 0, '0 ?? null ?? null');

x = undefined;
x = null ?? 0 ?? null;
assert.sameValue(x, 0, 'null ?? 0 ?? null');

x = undefined;
x = null ?? 0 ?? undefined;
assert.sameValue(x, 0, 'null ?? 0 ?? undefined');

x = undefined;
x = undefined ?? 0 ?? null;
assert.sameValue(x, 0, 'undefined ?? 0 ?? null');

x = undefined;
x = undefined ?? 0 ?? undefined;
assert.sameValue(x, 0, 'undefined ?? 0 ?? undefined');
