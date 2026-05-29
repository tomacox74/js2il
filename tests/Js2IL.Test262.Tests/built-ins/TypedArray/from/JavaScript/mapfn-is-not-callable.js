// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-%typedarray%.from
description: Throw a TypeError exception is mapfn is not callable
info: |
  22.2.2.1 %TypedArray%.from ( source [ , mapfn [ , thisArg ] ] )

  ...
  3. If mapfn was supplied and mapfn is not undefined, then
    a. If IsCallable(mapfn) is false, throw a TypeError exception.
  ...
includes: [testTypedArray.js]
features: [Symbol, Symbol.iterator, TypedArray]
---*/


function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

function compareArray(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    return false;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var TypedArray = Object.getPrototypeOf(Int8Array);

function testWithTypedArrayConstructors(fn) {
  var ctors = [Int8Array, Uint8Array, Int16Array, Int32Array, Float32Array, Float64Array];
  for (var i = 0; i < ctors.length; i++) {
    fn(ctors[i], function(value) { return value; });
  }
}

var getIterator = 0;
var arrayLike = {};
Object.defineProperty(arrayLike, Symbol.iterator, {
  get: function() {
    getIterator++;
  }
});

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, null);
}, "mapfn is null");

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, 42);
}, "mapfn is a number");

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, "");
}, "mapfn is a string");

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, {});
}, "mapfn is an ordinary object");

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, []);
}, "mapfn is an array");

assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, true);
}, "mapfn is a boolean");

var s = Symbol("1");
assert.throws(TypeError, function() {
  TypedArray.from(arrayLike, s);
}, "mapfn is a symbol");

assert.sameValue(
  getIterator, 0,
  "IsCallable(mapfn) check occurs before getting source[@@iterator]"
);
