// Copyright (c) 2014 Hank Yates. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.from
description: Testing Array.from when passed a String
author: Hank Yates (hankyates@gmail.com)
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

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var arrLikeSource = 'Test';
var result = Array.from(arrLikeSource);

assert.sameValue(result.length, 4, 'The value of result.length is expected to be 4');
assert.sameValue(result[0], 'T', 'The value of result[0] is expected to be "T"');
assert.sameValue(result[1], 'e', 'The value of result[1] is expected to be "e"');
assert.sameValue(result[2], 's', 'The value of result[2] is expected to be "s"');
assert.sameValue(result[3], 't', 'The value of result[3] is expected to be "t"');
