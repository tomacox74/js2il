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

var object;

object = {
  __proto__: undefined
};
assert.sameValue(
  Object.getPrototypeOf(object),
  Object.prototype,
  'prototype (undefined)'
);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'),
  undefined,
  'property (undefined)'
);

object = {
  __proto__: 1
};
assert.sameValue(
  Object.getPrototypeOf(object),
  Object.prototype,
  'prototype (numeric primitive)'
);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'),
  undefined,
  'property (numeric primitive)'
);

object = {
  __proto__: false
};
assert.sameValue(
  Object.getPrototypeOf(object),
  Object.prototype,
  'prototype (boolean primitive)'
);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'),
  undefined,
  'property (boolean primitive)'
);

object = {
  __proto__: 'string literal'
};
assert.sameValue(
  Object.getPrototypeOf(object),
  Object.prototype,
  'prototype (string primitive)'
);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'),
  undefined,
  'property (string primitive)'
);

object = {
  __proto__: Symbol('')
};
assert.sameValue(
  Object.getPrototypeOf(object),
  Object.prototype,
  'prototype (symbol)'
);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'),
  undefined,
  'property (symbol)'
);
