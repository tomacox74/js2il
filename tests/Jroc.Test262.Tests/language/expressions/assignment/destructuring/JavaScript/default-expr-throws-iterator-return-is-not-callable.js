// Copyright (C) 2025 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-runtime-semantics-destructuringassignmentevaluation
description: >
  Input throw-completion forwarded when IteratorClose returns abruptly because GetMethod throws.
info: |
  13.15.5.2 Runtime Semantics: DestructuringAssignmentEvaluation

  ArrayAssignmentPattern : [ AssignmentElementList , Elisionopt AssignmentRestElementopt ]
    ...
    2. Let status be Completion(IteratorDestructuringAssignmentEvaluation of AssignmentElementList with argument iteratorRecord).
    3. If status is an abrupt completion, then
      a. If iteratorRecord.[[Done]] is false, return ? IteratorClose(iteratorRecord, status).
      b. Return ? status.
    ...

  7.4.11 IteratorClose ( iteratorRecord, completion )
    ...
    3. Let innerResult be Completion(GetMethod(iterator, "return")).
    ...
    5. If completion is a throw completion, return ? completion.
    ...

  7.3.10 GetMethod ( V, P )
    1. Let func be ? GetV(V, P).
    2. If func is either undefined or null, return undefined.
    3. If IsCallable(func) is false, throw a TypeError exception.
    ...
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

function MyError() {}

function thrower() {
  throw new MyError();
}

for (var returnMethod of [0, 0n, true, "string", {}, Symbol()]) {
  var iterable = {
    [Symbol.iterator]() {
      return this;
    },
    next() {
      return {done: false};
    },
    return: returnMethod,
  };

  assert.throws(MyError, function() {
    var a;
    ([a = thrower()] = iterable);
  });
}
