// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = undefined;
x = true ?? 1;
assert.sameValue(x, true, 'true ?? null');

x = undefined;
x = true ?? null;
assert.sameValue(x, true, 'true ?? null');

x = undefined;
x = true ?? undefined;
assert.sameValue(x, true, 'true ?? undefined');

x = undefined;
x = true ?? null ?? undefined;
assert.sameValue(x, true, 'true ?? null ?? undefined');

x = undefined;
x = true ?? undefined ?? null;
assert.sameValue(x, true, 'true ?? undefined ?? null');

x = undefined;
x = true ?? null ?? null;
assert.sameValue(x, true, 'true ?? null ?? null');

x = undefined;
x = true ?? undefined ?? undefined;
assert.sameValue(x, true, 'true ?? null ?? null');

x = undefined;
x = null ?? true ?? null;
assert.sameValue(x, true, 'null ?? true ?? null');

x = undefined;
x = null ?? true ?? undefined;
assert.sameValue(x, true, 'null ?? true ?? undefined');

x = undefined;
x = undefined ?? true ?? null;
assert.sameValue(x, true, 'undefined ?? true ?? null');

x = undefined;
x = undefined ?? true ?? undefined;
assert.sameValue(x, true, 'undefined ?? true ?? undefined');
