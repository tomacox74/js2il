// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5
description: >
    class default constructor arguments
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message === undefined ? '' : String(message);
}
function __test262SameValue(a, b) {
  return Object.is(a, b);
}
function compareArray(actual, expected) {
  if (!actual || !expected || actual.length !== expected.length) {
    return false;
  }
  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }
  return true;
}
function verifyProperty(obj, name, desc) {
  const actual = Object.getOwnPropertyDescriptor(obj, name);
  let ok = !!actual;
  if ('value' in desc) ok = ok && Object.is(actual.value, desc.value);
  if ('writable' in desc) ok = ok && actual.writable === desc.writable;
  if ('enumerable' in desc) ok = ok && actual.enumerable === desc.enumerable;
  if ('configurable' in desc) ok = ok && actual.configurable === desc.configurable;
  if ('get' in desc) ok = ok && actual.get === desc.get;
  if ('set' in desc) ok = ok && actual.set === desc.set;
  console.log(ok);
  return ok;
}
var assert = function assert(condition) {
  console.log(!!condition);
};
assert.sameValue = function(actual, expected) {
  console.log(__test262SameValue(actual, expected));
};
assert.notSameValue = function(actual, unexpected) {
  console.log(!__test262SameValue(actual, unexpected));
};
assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};
assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

var args, that;
class Base {
  constructor() {
    that = this;
    args = arguments;
  }
}
class Derived extends Base {}

new Derived;
assert.sameValue(args.length, 0, "The value of `args.length` is `0`");

new Derived(0, 1, 2);
assert.sameValue(args.length, 3, "The value of `args.length` is `3`");
assert.sameValue(
  that instanceof Derived,
  true,
  "The result of `that instanceof Derived` is `true`"
);

var arr = new Array(100);
var obj = {};
assert.throws(TypeError, function() {Derived.apply(obj, arr);});

