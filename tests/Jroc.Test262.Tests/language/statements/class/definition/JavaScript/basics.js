// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5
description: >
    class basics
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

var C = class C {}
assert.sameValue(typeof C, 'function', "`typeof C` is `'function'`");
assert.sameValue(
    Object.getPrototypeOf(C.prototype),
    Object.prototype,
    "`Object.getPrototypeOf(C.prototype)` returns `Object.prototype`"
);
assert.sameValue(
    Object.getPrototypeOf(C),
    Function.prototype,
    "`Object.getPrototypeOf(C)` returns `Function.prototype`"
);
assert.sameValue(C.name, 'C', "The value of `C.name` is `'C'`");

class D {}
assert.sameValue(typeof D, 'function', "`typeof D` is `'function'`");
assert.sameValue(
    Object.getPrototypeOf(D.prototype),
    Object.prototype,
    "`Object.getPrototypeOf(D.prototype)` returns `Object.prototype`"
);
assert.sameValue(
    Object.getPrototypeOf(D),
    Function.prototype,
    "`Object.getPrototypeOf(D)` returns `Function.prototype`"
);
assert.sameValue(D.name, 'D', "The value of `D.name` is `'D'`");

class D2 { constructor() {} }
assert.sameValue(D2.name, 'D2', "The value of `D2.name` is `'D2'`");

var E = class {}
assert.sameValue(E.name, 'E', "The value of `E.name` is `'E'`");

var F = class { constructor() {} };
assert.sameValue(F.name, 'F', "The value of `F.name` is `'F'`");

