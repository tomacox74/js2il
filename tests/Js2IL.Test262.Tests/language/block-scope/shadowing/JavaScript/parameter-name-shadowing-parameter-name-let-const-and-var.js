// Copyright (C) 2011 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 13.1
description: >
    parameter name shadowing parameter name, let, const and var
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

function fn(a) {
  let b = 1;
  var c = 1;
  const d = 1;

  (function(a, b, c, d) {
    a = 2;
    b = 2;
    c = 2;
    d = 2;

    assert.sameValue(a, 2);
    assert.sameValue(b, 2);
    assert.sameValue(c, 2);
    assert.sameValue(d, 2);
  })(1, 1);

  assert.sameValue(a, 1);
  assert.sameValue(b, 1);
  assert.sameValue(c, 1);
  assert.sameValue(d, 1);
}

fn(1);
