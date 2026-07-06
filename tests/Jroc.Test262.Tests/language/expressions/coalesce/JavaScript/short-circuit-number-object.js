// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x;
var obj = {
    toString() {
        return null;
    },
    valueOf() {
        return null;
    }
};

x = undefined;
x = obj ?? 1;
assert.sameValue(x, obj, 'obj ?? 1');

x = undefined;
x = obj ?? null;
assert.sameValue(x, obj, 'obj ?? null');

x = undefined;
x = obj ?? undefined;
assert.sameValue(x, obj, 'obj ?? undefined');

x = undefined;
x = obj ?? null ?? undefined;
assert.sameValue(x, obj, 'obj ?? null ?? undefined');

x = undefined;
x = obj ?? undefined ?? null;
assert.sameValue(x, obj, 'obj ?? undefined ?? null');

x = undefined;
x = obj ?? null ?? null;
assert.sameValue(x, obj, 'obj ?? null ?? null');

x = undefined;
x = obj ?? undefined ?? undefined;
assert.sameValue(x, obj, 'obj ?? null ?? null');

x = undefined;
x = null ?? obj ?? null;
assert.sameValue(x, obj, 'null ?? obj ?? null');

x = undefined;
x = null ?? obj ?? undefined;
assert.sameValue(x, obj, 'null ?? obj ?? undefined');

x = undefined;
x = undefined ?? obj ?? null;
assert.sameValue(x, obj, 'undefined ?? obj ?? null');

x = undefined;
x = undefined ?? obj ?? undefined;
assert.sameValue(x, obj, 'undefined ?? obj ?? undefined');
