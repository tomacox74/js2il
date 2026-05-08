// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.6.4
description: >
    Mapped arguments object mutation via alias during traversal using for..of
info: |
    "Mapped" arguments objects should be able to be traversed using a `for..of`
    loop, and dynamic changes to the formal parameters should be reflected in
    the iterated values.
flags: [noStrict]
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

var expected = [1, 3, 1];
var i = 0;

(function(a, b, c) {
  for (var value of arguments) {
    a = b;
    b = c;
    c = i;
    assert.sameValue(value, expected[i], 'argument at index ' + i);
    i++;
  }

}(1, 2, 3));

assert.sameValue(i, 3, 'Visits all arguments');
