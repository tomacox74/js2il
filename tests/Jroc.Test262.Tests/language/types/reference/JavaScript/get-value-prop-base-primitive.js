// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-getvalue
es6id: 6.2.3.1
description: >
  When the base of a property reference is primitive, it is coerced to an
  object during value retrieval
info: |
  [...]
  5. If IsPropertyReference(V) is true, then
     a. If HasPrimitiveBase(V) is true, then
        i. Assert: In this case, base will never be null or undefined.
        ii. Let base be ToObject(base).
     b. Return ? base.[[Get]](GetReferencedName(V), GetThisValue(V)).
features: [Symbol]
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

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
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

Number.prototype.test262 = 'number prototype';
assert.sameValue(1..test262, 'number prototype');

String.prototype.test262 = 'string prototype';
assert.sameValue(''.test262, 'string prototype');

Boolean.prototype.test262 = 'Boolean prototype';
assert.sameValue(true.test262, 'Boolean prototype');

Symbol.prototype.test262 = 'Symbol prototype';
assert.sameValue(Symbol().test262, 'Symbol prototype');
