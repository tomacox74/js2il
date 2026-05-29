// Copyright 2021 Jamie Kyle.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.hasown
info: |
    Object.hasOwn ( _O_, _P_ )

    1. Let _obj_ be ? ToObject(_O_).
    2. Let _key_ be ? ToPropertyKey(_P_).
    3. Return ? HasOwnProperty(_obj_, _key_).
description: >
    Checking type of the Object.hasOwn and the returned result
author: Jamie Kyle
features: [Object.hasOwn]
---*/


// test262 execution-port helpers
function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

assert.sameValue(typeof Object.hasOwn, 'function');
assert(Object.hasOwn(Object, 'hasOwn'));
