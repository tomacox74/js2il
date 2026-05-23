// Copyright (C) 2015 Caitlin Potter. All rights reserved.
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

var BaseClass = class {};

assert.sameValue(
  BaseClass.hasOwnProperty('caller'), false, 'No "caller" own property'
);
assert.sameValue(
  BaseClass.hasOwnProperty('arguments'), false, 'No "arguments" own property'
);

assert.throws(TypeError, function() {
  return BaseClass.caller;
});

assert.throws(TypeError, function() {
  BaseClass.caller = {};
});

assert.throws(TypeError, function() {
  return BaseClass.arguments;
});

assert.throws(TypeError, function() {
  BaseClass.arguments = {};
});

var SubClass = class extends BaseClass {};

assert.sameValue(
  SubClass.hasOwnProperty('caller'), false, 'No "caller" own property'
);
assert.sameValue(
  SubClass.hasOwnProperty('arguments'), false, 'No "arguments" own property'
);

assert.throws(TypeError, function() {
  return SubClass.caller;
});

assert.throws(TypeError, function() {
  SubClass.caller = {};
});

assert.throws(TypeError, function() {
  return SubClass.arguments;
});

assert.throws(TypeError, function() {
  SubClass.arguments = {};
});
