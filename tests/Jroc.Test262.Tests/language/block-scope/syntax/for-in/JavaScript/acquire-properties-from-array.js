// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.1
description: >
    for-in to acquire properties from array
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

function props(x) {
  var array = [];
  for (let p in x) array.push(p);
  return array;
}
var subject;

subject = props([]);
assert.sameValue(subject.length, 0, "[]: length");
assert.sameValue(subject[0], undefined, "[]: first property");
assert.sameValue(subject[1], undefined, "[]: second property");
assert.sameValue(subject[2], undefined, "[]: third property");
assert.sameValue(subject[3], undefined, "[]: fourth property");

subject = props([1]);
assert.sameValue(subject.length, 1, "[1]: length");
assert.sameValue(subject[0], "0", "[1]: first property");
assert.sameValue(subject[1], undefined, "[1]: second property");
assert.sameValue(subject[2], undefined, "[1]: third property");
assert.sameValue(subject[3], undefined, "[1]: fourth property");

subject = props([1, 2]);
assert.sameValue(subject.length, 2, "[1, 2]: length");
assert.sameValue(subject[0], "0", "[1, 2]: first property");
assert.sameValue(subject[1], "1", "[1, 2]: second property");
assert.sameValue(subject[2], undefined, "[1, 2]: third property");
assert.sameValue(subject[3], undefined, "[1, 2]: fourth property");

subject = props([1, 2, 3]);
assert.sameValue(subject.length, 3, "[1, 2, 3]: length");
assert.sameValue(subject[0], "0", "[1, 2, 3]: first property");
assert.sameValue(subject[1], "1", "[1, 2, 3]: second property");
assert.sameValue(subject[2], "2", "[1, 2, 3]: third property");
assert.sameValue(subject[3], undefined, "[1, 2, 3]: fourth property");
