// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.1
description: >
    Rest parameter and Function.prototype.apply
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

function af(...a) {
  return a.length;
}

assert.sameValue(af.apply(null, []), 0, "`af.apply(null, [])` returns `0`");
assert.sameValue(af.apply(null, [1]), 1, "`af.apply(null, [1])` returns `1`");
assert.sameValue(af.apply(null, [1, 2]), 2, "`af.apply(null, [1, 2])` returns `2`");
assert.sameValue(af.apply(null, [1, ,2]), 3, "`af.apply(null, [1, ,2])` returns `3`");
