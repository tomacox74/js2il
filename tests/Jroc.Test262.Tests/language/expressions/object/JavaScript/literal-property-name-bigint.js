// Copyright (C) 2020 Igalia S.L, Toru Nagashima. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

// Property

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

let o = { 999999999999999999n: true }; // greater than max safe integer

assert.sameValue(o["999999999999999999"], true,
    "the property name must be the string representation of the numeric value.");

// MethodDeclaration

o = { 1n() { return "bar"; } };
assert.sameValue(o["1"](), "bar",
    "the property name must be the string representation of the numeric value.");

class C {
  1n() { return "baz"; }
}

let c = new C();
assert.sameValue(c["1"](), "baz",
    "the property name must be the string representation of the numeric value.");

// Destructuring

let { 1n: a } = { "1": "foo" };
assert.sameValue(a, "foo",
    "the property name must be the string representation of the numeric value.");
