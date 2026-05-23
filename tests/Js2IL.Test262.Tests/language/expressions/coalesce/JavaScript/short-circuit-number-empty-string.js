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
var str = '';

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
