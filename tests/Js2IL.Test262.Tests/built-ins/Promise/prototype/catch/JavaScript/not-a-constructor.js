// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-ecmascript-standard-built-in-objects
description: >
  Promise.prototype.catch does not implement [[Construct]], is not new-able
info: |
  ECMAScript Function Objects

  Built-in function objects that are not identified as constructors do not
  implement the [[Construct]] internal method unless otherwise specified in
  the description of a particular function.

  sec-evaluatenew

  ...
  7. If IsConstructor(constructor) is false, throw a TypeError exception.
  ...
includes: [isConstructor.js]
features: [Reflect.construct, arrow-function]
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

assert.sameValue(
  isConstructor(Promise.prototype.catch),
  false,
  'isConstructor(Promise.prototype.catch) must return false'
);

assert.throws(TypeError, () => {
  let p = new Promise(() => {}); new p.catch();
});

