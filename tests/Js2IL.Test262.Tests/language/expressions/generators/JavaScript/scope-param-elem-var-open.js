// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-functiondeclarationinstantiation
description: >
    sloppy direct evals in params introduce vars
info: |
    [...]
    20. Else,
      a. NOTE: A separate Environment Record is needed to ensure that bindings created by direct eval calls in the formal parameter list are outside the environment where parameters are declared.
      b. Let calleeEnv be the LexicalEnvironment of calleeContext.
      c. Let env be NewDeclarativeEnvironment(calleeEnv).
      d. Let envRec be env's EnvironmentRecord.
    [...]
flags: [noStrict]
features: [generators]
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

assert.compareArray = function(actual, expected, message) {
  var passed = Array.isArray(actual) && Array.isArray(expected) && actual.length === expected.length;
  if (passed) {
    for (var i = 0; i < actual.length; i++) {
      if (!__sameValue(actual[i], expected[i])) {
        passed = false;
        break;
      }
    }
  }
  __assertResult(passed, message || 'Expected arrays to compare equal');
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

var x = 'outside';
var probe1, probe2;

(function*(
    _ = probe1 = function() { return x; },
    __ = (eval('var x = "inside";'), probe2 = function() { return x; })
  ) {
}().next());

assert.sameValue(probe1(), 'inside');
assert.sameValue(probe2(), 'inside');
