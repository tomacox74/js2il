// Copyright (C) 2020 Alexey Shvayka. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-properties-of-asyncgeneratorfunction-prototype
description: >
  %AsyncGeneratorFunction.prototype% is an ordinary non-callable object.
info: |
  Properties of the AsyncGeneratorFunction Prototype Object

  The AsyncGeneratorFunction prototype object:

  [...]
  * is an ordinary object.
  * is not a function object and does not have an [[ECMAScriptCode]] internal slot
    or any other of the internal slots listed in Table 28 or Table 75.
features: [async-iteration]
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
  throw new Test262Error(message);
}

function $DONE(error) {
  if (error) {
    throw error;
  }
}

function __sameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __assertResult(passed, message) {
  console.log(!!passed);
  if (!passed) {
    throw new Error(message || 'Assertion failed');
  }
}

function assert(condition, message) {
  __assertResult(!!condition, message);
}

assert.sameValue = function(actual, expected, message) {
  __assertResult(__sameValue(actual, expected), message || 'Expected SameValue');
};

assert.notSameValue = function(actual, unexpected, message) {
  __assertResult(!__sameValue(actual, unexpected), message || 'Expected values to differ');
};

assert.throws = function(expectedErrorConstructor, fn, message) {
  var passed = false;
  try {
    fn();
  } catch (error) {
    passed = error instanceof expectedErrorConstructor ||
      (error && error.constructor === expectedErrorConstructor) ||
      (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
  }
  __assertResult(passed, message || 'Expected function to throw');
};

function verifyProperty(object, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(object, name);
  var passed = !!actual;

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
    passed = __sameValue(actual.value, desc.value);
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    passed = actual.writable === desc.writable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    passed = actual.enumerable === desc.enumerable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    passed = actual.configurable === desc.configurable;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
    passed = actual.get === desc.get;
  }
  if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
    passed = actual.set === desc.set;
  }

  __assertResult(passed, 'verifyProperty failed for ' + name);
}

function checkSequence(sequence, message) {
  var passed = true;
  for (var i = 0; i < sequence.length; i++) {
    if (sequence[i] !== i + 1) {
      passed = false;
      break;
    }
  }
  __assertResult(passed, message || 'Unexpected callback sequence');
}

function isConstructor(value) {
  try {
    Reflect.construct(Function, [], value);
    return true;
  } catch (error) {
    return false;
  }
}

function getWellKnownIntrinsicObject(name) {
  if (name === '%AsyncGeneratorFunction%') {
    return Object.getPrototypeOf(async function* () {}).constructor;
  }
  throw new Error('Unsupported intrinsic: ' + name);
}

var AsyncGeneratorFunctionPrototype = Object.getPrototypeOf(async function* () {});

assert.sameValue(typeof AsyncGeneratorFunctionPrototype, "object");
assert.throws(TypeError, function() {
  AsyncGeneratorFunctionPrototype();
});

assert(!AsyncGeneratorFunctionPrototype.hasOwnProperty("length"), "length");
assert(!AsyncGeneratorFunctionPrototype.hasOwnProperty("name"), "name");
