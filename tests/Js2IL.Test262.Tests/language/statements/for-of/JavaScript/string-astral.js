// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: String traversal using for..of (astral symbols)
info: |
    String literals should be able to be traversed using a `for...of` loop. The
    loop body should execute once for each astral symbol.
es6id: 13.6.4
---*/

function Test262Error(message) {
  this.name = "Test262Error";
  this.message = message || "";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };
assert.compareArray = function(actual, expected) {
  var sameLength = Array.isArray(actual) && Array.isArray(expected) && actual.length === expected.length;
  var sameValues = sameLength;
  for (var i = 0; i < actual.length && sameValues; i++) {
    sameValues = Object.is(actual[i], expected[i]);
  }
  console.log(sameLength && sameValues);
};
assert.throws = function(expectedErrorConstructor, func) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};
function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  console.log(actual !== undefined);
  if ('value' in desc) {
    console.log(Object.is(actual && actual.value, desc.value));
  }
  if ('writable' in desc) {
    console.log(!!actual && actual.writable === desc.writable);
  }
  if ('enumerable' in desc) {
    console.log(!!actual && actual.enumerable === desc.enumerable);
  }
  if ('configurable' in desc) {
    console.log(!!actual && actual.configurable === desc.configurable);
  }
}
function isConstructor(argument) {
  if ((typeof argument !== 'function' && typeof argument !== 'object') || argument === null) {
    return false;
  }
  try {
    Reflect.construct(function() {}, [], argument);
    return true;
  } catch (error) {
    return false;
  }
}

var string = 'a\ud801\udc28b\ud801\udc28';
var first = 'a';
var second = '𐐨';
var third = 'b';
var fourth = '𐐨';

var iterationCount = 0;

for (var value of string) {
  assert.sameValue(value, first);
  first = second;
  second = third;
  third = fourth;
  fourth = null;
  iterationCount += 1;
}

assert.sameValue(iterationCount, 4);
