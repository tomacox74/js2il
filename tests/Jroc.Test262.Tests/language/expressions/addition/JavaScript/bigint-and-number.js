// Copyright (C) 2018 Igalia, S.L. All rights reserved.
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

assert.throws(TypeError, function() {
  1n + 1;
}, '1n + 1 throws TypeError');

assert.throws(TypeError, function() {
  1 + 1n;
}, '1 + 1n throws TypeError');

assert.throws(TypeError, function() {
  Object(1n) + 1;
}, 'Object(1n) + 1 throws TypeError');

assert.throws(TypeError, function() {
  1 + Object(1n);
}, '1 + Object(1n) throws TypeError');

assert.throws(TypeError, function() {
  1n + Object(1);
}, '1n + Object(1) throws TypeError');

assert.throws(TypeError, function() {
  Object(1) + 1n;
}, 'Object(1) + 1n throws TypeError');

assert.throws(TypeError, function() {
  Object(1n) + Object(1);
}, 'Object(1n) + Object(1) throws TypeError');

assert.throws(TypeError, function() {
  Object(1) + Object(1n);
}, 'Object(1) + Object(1n) throws TypeError');

assert.throws(TypeError, function() {
  1n + NaN;
}, '1n + NaN throws TypeError');

assert.throws(TypeError, function() {
  NaN + 1n;
}, 'NaN + 1n throws TypeError');

assert.throws(TypeError, function() {
  1n + Infinity;
}, '1n + Infinity throws TypeError');

assert.throws(TypeError, function() {
  Infinity + 1n;
}, 'Infinity + 1n throws TypeError');

assert.throws(TypeError, function() {
  1n + true;
}, '1n + true throws TypeError');

assert.throws(TypeError, function() {
  true + 1n;
}, 'true + 1n throws TypeError');

assert.throws(TypeError, function() {
  1n + null;
}, '1n + null throws TypeError');

assert.throws(TypeError, function() {
  null + 1n;
}, 'null + 1n throws TypeError');

assert.throws(TypeError, function() {
  1n + undefined;
}, '1n + undefined throws TypeError');

assert.throws(TypeError, function() {
  undefined + 1n;
}, 'undefined + 1n throws TypeError');
