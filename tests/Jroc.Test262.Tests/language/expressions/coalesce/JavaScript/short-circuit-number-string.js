// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;
var str = 'undefined';

x = undefined;
x = str ?? 1;
assert.sameValue(x, str, 'str ?? 1');

x = undefined;
x = str ?? null;
assert.sameValue(x, str, 'str ?? null');

x = undefined;
x = str ?? undefined;
assert.sameValue(x, str, 'str ?? undefined');

x = undefined;
x = str ?? null ?? undefined;
assert.sameValue(x, str, 'str ?? null ?? undefined');

x = undefined;
x = str ?? undefined ?? null;
assert.sameValue(x, str, 'str ?? undefined ?? null');

x = undefined;
x = str ?? null ?? null;
assert.sameValue(x, str, 'str ?? null ?? null');

x = undefined;
x = str ?? undefined ?? undefined;
assert.sameValue(x, str, 'str ?? null ?? null');

x = undefined;
x = null ?? str ?? null;
assert.sameValue(x, str, 'null ?? str ?? null');

x = undefined;
x = null ?? str ?? undefined;
assert.sameValue(x, str, 'null ?? str ?? undefined');

x = undefined;
x = undefined ?? str ?? null;
assert.sameValue(x, str, 'undefined ?? str ?? null');

x = undefined;
x = undefined ?? str ?? undefined;
assert.sameValue(x, str, 'undefined ?? str ?? undefined');
