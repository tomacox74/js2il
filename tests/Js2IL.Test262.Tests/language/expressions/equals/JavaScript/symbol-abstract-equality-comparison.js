// Copyright (C) 2013 the V8 project authors. All rights reserved.
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

var symA = Symbol('66');
var symB = Symbol('66');

assert.sameValue(symA == symA, true, "The result of `symA == symA` is `true`");
assert.sameValue(symA == symA.valueOf(), true, "The result of `symA == symA.valueOf()` is `true`");
assert.sameValue(symA.valueOf() == symA, true, "The result of `symA.valueOf() == symA` is `true`");

assert.sameValue(symB == symB, true, "The result of `symB == symB` is `true`");
assert.sameValue(symB == symB.valueOf(), true, "The result of `symB == symB.valueOf()` is `true`");
assert.sameValue(symB.valueOf() == symB, true, "The result of `symB.valueOf() == symB` is `true`");

assert.sameValue(symA == symB, false, "The result of `symA == symB` is `false`");
