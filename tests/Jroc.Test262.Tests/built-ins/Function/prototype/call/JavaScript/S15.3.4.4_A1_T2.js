// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The call method performs a function call using the [[Call]] property of
    the object. If the object does not have a [[Call]] property, a TypeError
    exception is thrown
es5id: 15.3.4.4_A1_T2
description: >
    Calling "call" method of the object that does not have a [[Call]]
    property.  Prototype of the object is Function.prototype
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

function FACTORY() {}

FACTORY.prototype = Function.prototype;

var obj = new FACTORY;

assert.sameValue(typeof obj.call, "function", 'The value of `typeof obj.call` is expected to be "function"');

try {
  obj.call();
  throw new Test262Error('#2: If the object does not have a [[Call]] property, a TypeError exception is thrown');
} catch (e) {
  assert(e instanceof TypeError, 'The result of evaluating (e instanceof TypeError) is expected to be true');
}

