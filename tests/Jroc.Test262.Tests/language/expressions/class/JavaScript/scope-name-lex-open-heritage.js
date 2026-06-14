// Copyright (C) 2016 the V8 project authors. All rights reserved.
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

var probeBefore = function() { return C; };
var probeHeritage, setHeritage;
var C = 'outside';

var cls = class C extends (
    probeHeritage = function() { return C; },
    setHeritage = function() { C = null; }
  ) {
  method() {
    return C;
  }
};

assert.sameValue(probeBefore(), 'outside');
assert.sameValue(probeHeritage(), cls, 'from class heritage');
assert.throws(TypeError, setHeritage, 'inner binding rejects modification');
assert.sameValue(probeHeritage(), cls, 'inner binding is immutable');
assert.sameValue(cls.prototype.method(), cls, 'from instance method');
