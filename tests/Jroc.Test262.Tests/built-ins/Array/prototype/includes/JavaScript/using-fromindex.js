// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.includes
description: Searches using fromIndex
info: |
  22.1.3.11 Array.prototype.includes ( searchElement [ , fromIndex ] )

  ...
  5. If n ≥ 0, then
    a. Let k be n.
  6. Else n < 0,
    a. Let k be len + n.
    b. If k < 0, let k be 0.
  7. Repeat, while k < len
    a. Let elementK be the result of ? Get(O, ! ToString(k)).
    b. If SameValueZero(searchElement, elementK) is true, return true.
    c. Increase k by 1.
  ...
features: [Array.prototype.includes]
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

var sample = ["a", "b", "c"];
assert.sameValue(sample.includes("a", 0), true, "includes('a', 0)");
assert.sameValue(sample.includes("a", 1), false, "includes('a', 1)");
assert.sameValue(sample.includes("a", 2), false, "includes('a', 2)");

assert.sameValue(sample.includes("b", 0), true, "includes('b', 0)");
assert.sameValue(sample.includes("b", 1), true, "includes('b', 1)");
assert.sameValue(sample.includes("b", 2), false, "includes('b', 2)");

assert.sameValue(sample.includes("c", 0), true, "includes('c', 0)");
assert.sameValue(sample.includes("c", 1), true, "includes('c', 1)");
assert.sameValue(sample.includes("c", 2), true, "includes('c', 2)");

assert.sameValue(sample.includes("a", -1), false, "includes('a', -1)");
assert.sameValue(sample.includes("a", -2), false, "includes('a', -2)");
assert.sameValue(sample.includes("a", -3), true, "includes('a', -3)");
assert.sameValue(sample.includes("a", -4), true, "includes('a', -4)");

assert.sameValue(sample.includes("b", -1), false, "includes('b', -1)");
assert.sameValue(sample.includes("b", -2), true, "includes('b', -2)");
assert.sameValue(sample.includes("b", -3), true, "includes('b', -3)");
assert.sameValue(sample.includes("b", -4), true, "includes('b', -4)");

assert.sameValue(sample.includes("c", -1), true, "includes('c', -1)");
assert.sameValue(sample.includes("c", -2), true, "includes('c', -2)");
assert.sameValue(sample.includes("c", -3), true, "includes('c', -3)");
assert.sameValue(sample.includes("c", -4), true, "includes('c', -4)");
