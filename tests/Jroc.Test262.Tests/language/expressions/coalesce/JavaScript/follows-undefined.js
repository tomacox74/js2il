// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = undefined ?? 42;
assert.sameValue(x, 42, 'undefined ?? 42');

x = undefined ?? undefined;
assert.sameValue(x, undefined, 'undefined ?? undefined');

x = undefined ?? null;
assert.sameValue(x, null, 'undefined ?? null');

x = undefined ?? false;
assert.sameValue(x, false, 'undefined ?? false');
