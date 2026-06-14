// Copyright (C) 2019 Caio Lima (Igalia SL). All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: If an initializer returns an abrupt completion, other initializers should not execute
esid: sec-ecmascript-function-objects-construct-argumentslist-newtarget
info: |
  [[Construct]] ( argumentsList, newTarget)
    ...
    8. If kind is "base", then
      a. Perform OrdinaryCallBindThis(F, calleeContext, thisArgument).
      b. Let result be InitializeInstanceFields(thisArgument, F).
      c. If result is an abrupt completion, then
        i. Remove calleeContext from execution context stack and restore callerContext as the running execution context.
        ii. Return Completion(result).
    ...

  ClassTail : ClassHeritage { ClassBody }
    ...
    34. For each item fieldRecord in order from staticFields,
      a. Perform ? DefineField(F, field).
    ...

features: [class-fields-public, class-static-fields-public, class]
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

function abruptCompletion() {
  throw new Test262Error();
}

let neverExecuted = false;

function sideEffect() {
  neverExecuted = true;
}

class C {
  a = abruptCompletion();
  b = sideEffect();
}

assert.throws(Test262Error, function() {
  let c = new C();
}, 'field initializer should end with abrupt completion');
assert.sameValue(neverExecuted, false);

assert.throws(Test262Error, function() {
  class D {
    static a = abruptCompletion();
    static b = sideEffect();
  }
}, 'static field initializer should end with abrupt completion');
assert.sameValue(neverExecuted, false);
