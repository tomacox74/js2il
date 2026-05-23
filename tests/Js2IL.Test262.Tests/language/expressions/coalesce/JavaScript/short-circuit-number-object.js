// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.sameValue = function(actual, expected) {
    console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected) {
    console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func) {
    try {
        func();
        console.log(false);
    } catch (error) {
        console.log(error instanceof expectedErrorConstructor || error.constructor === expectedErrorConstructor);
    }
};
assert.compareArray = function(actual, expected) {
    console.log(actual.length === expected.length && actual.every(function(value, index) {
        return Object.is(value, expected[index]);
    }));
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
