// Copyright 2011 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If IsCallable(func) is false, then throw a TypeError exception.
es5id: 15.3.4.4_A16
description: >
    A RegExp is not a function, but it may be callable. Iff it is,
    it's typeof should be 'function', in which case call should accept
    it as a valid this value.
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

var re = (/x/);
if (typeof re === 'function') {
  Function.prototype.call.call(re, undefined, 'x');
} else {
  try {
    Function.prototype.bind.call(re, undefined);
    throw new Test262Error('#1: If IsCallable(func) is false, ' +
      'then (bind should) throw a TypeError exception');
  } catch (e) {
    assert(e instanceof TypeError, 'The result of evaluating (e instanceof TypeError) is expected to be true');
  }
}

