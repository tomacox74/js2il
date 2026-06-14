// Copyright (C) 2024 Kevin Gibbons. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-uint8array.frombase64
description: Uint8Array.fromBase64 throws a SyntaxError when input has non-base64, non-ascii-whitespace characters
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

var illegal = [
  'Zm.9v',
  'Zm9v^',
  'Zg==&',
  'Z−==', // U+2212 'Minus Sign'
  'Z＋==', // U+FF0B 'Fullwidth Plus Sign'
  'Zg\u00A0==', // nbsp
  'Zg\u2009==', // thin space
  'Zg\u2028==', // line separator
];
illegal.forEach(function(value) {
  assert.throws(SyntaxError, function() {
    Uint8Array.fromBase64(value)
  });
});
