// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;

x = null ?? 42;
assert.sameValue(x, 42, 'null ?? 42');

x = null ?? undefined;
assert.sameValue(x, undefined, 'null ?? undefined');

x = null ?? null;
assert.sameValue(x, null, 'null ?? null');

x = null ?? false;
assert.sameValue(x, false, 'null ?? false');
