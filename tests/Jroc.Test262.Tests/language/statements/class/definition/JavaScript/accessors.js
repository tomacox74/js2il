// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5
description: >
    class accessors
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

function verifyProperty(obj, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(obj, name);
  if (actual === undefined) {
    console.log(false);
    return;
  }

  var ok = true;

  if (Object.prototype.hasOwnProperty.call(desc, 'value')) {
    ok = ok && Object.is(actual.value, desc.value);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    ok = ok && Object.is(actual.writable, desc.writable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    ok = ok && Object.is(actual.enumerable, desc.enumerable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    ok = ok && Object.is(actual.configurable, desc.configurable);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'get')) {
    ok = ok && Object.is(actual.get, desc.get);
  }

  if (Object.prototype.hasOwnProperty.call(desc, 'set')) {
    ok = ok && Object.is(actual.set, desc.set);
  }

  console.log(ok);
}

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function assertAccessorDescriptor(object, name) {
  var desc = Object.getOwnPropertyDescriptor(object, name);
  assert.sameValue(desc.configurable, true, "The value of `desc.configurable` is `true`");
  assert.sameValue(desc.enumerable, false, "The value of `desc.enumerable` is `false`");
  assert.sameValue(typeof desc.get, 'function', "`typeof desc.get` is `'function'`");
  assert.sameValue(typeof desc.set, 'function', "`typeof desc.set` is `'function'`");
  assert.sameValue(
    'prototype' in desc.get,
    false,
    "The result of `'prototype' in desc.get` is `false`"
  );
  assert.sameValue(
    'prototype' in desc.set,
    false,
    "The result of `'prototype' in desc.set` is `false`"
  );
}


class C {
  constructor(x) {
    this._x = x;
  }

  get x() { return this._x; }
  set x(v) { this._x = v; }

  static get staticX() { return this._x; }
  static set staticX(v) { this._x = v; }
}

assertAccessorDescriptor(C.prototype, 'x');
assertAccessorDescriptor(C, 'staticX');

var c = new C(1);
c._x = 1;
assert.sameValue(c.x, 1, "The value of `c.x` is `1`, after executing `c._x = 1;`");
c.x = 2;
assert.sameValue(c._x, 2, "The value of `c._x` is `2`, after executing `c.x = 2;`");

C._x = 3;
assert.sameValue(C.staticX, 3, "The value of `C.staticX` is `3`, after executing `C._x = 3;`");
C._x = 4;
assert.sameValue(C.staticX, 4, "The value of `C.staticX` is `4`, after executing `C._x = 4;`");
