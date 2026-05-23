// Copyright (C) 2015 the V8 project authors. All rights reserved.
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

var y = {};
var retVal;

y[Symbol.toPrimitive] = function() {
  return retVal;
};

retVal = 86;
assert.sameValue(0 == y, false, 'number primitive (not equal)');
assert.sameValue(86 == y, true, 'number primitive (equal)');

retVal = 'str';
assert.sameValue(0 == y, false, 'string primitive (not equal)');
assert.sameValue('str' == y, true, 'sting primitive (equal)');

retVal = Symbol.toPrimitive;
assert.sameValue(0 == y, false, 'symbol (not equal)');
assert.sameValue(Symbol.toPrimitive == y, true, 'symbol (equal)');
