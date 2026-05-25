// Copyright (C) 2024 Kevin Gibbons. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-uint8array.frombase64
description: Uint8Array.fromBase64 throws if its argument is not a string
features: [uint8array-base64, TypedArray]
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

var toStringCalls = 0;
var throwyToString = {
  toString: function() {
    toStringCalls += 1;
    throw new Test262Error("toString called");
  }
};

assert.throws(TypeError, function() {
  Uint8Array.fromBase64(throwyToString);
});
assert.sameValue(toStringCalls, 0);


var optionAccesses = 0;
var touchyOptions = {};
Object.defineProperty(touchyOptions, "alphabet", {
  get: function() {
    optionAccesses += 1;
    throw new Test262Error("alphabet accessed");
  }
});
Object.defineProperty(touchyOptions, "lastChunkHandling", {
  get: function() {
    optionAccesses += 1;
    throw new Test262Error("lastChunkHandling accessed");
  }
});
assert.throws(TypeError, function() {
  Uint8Array.fromBase64(throwyToString, touchyOptions);
});
assert.sameValue(toStringCalls, 0);
assert.sameValue(optionAccesses, 0);
