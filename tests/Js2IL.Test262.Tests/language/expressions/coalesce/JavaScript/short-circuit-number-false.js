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
