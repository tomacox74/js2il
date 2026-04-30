// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Sanity test for throw statement
es5id: 12.13_A1
description: Trying to throw exception with "throw"
---*/

function assert(value, message) {
  console.log(!!value);
}
assert.sameValue = function(actual, expected, message) {
  console.log(Object.is(actual, expected));
};
assert.notSameValue = function(actual, unexpected, message) {
  console.log(!Object.is(actual, unexpected));
};
assert.throws = function(expectedErrorConstructor, func, message) {
  try {
    func();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedErrorConstructor);
  }
};

var inCatch = false;

try {
  throw "expected_message";
} catch (err) {
  assert.sameValue(err, "expected_message");
  inCatch = true;
}

assert.sameValue(inCatch, true);

console.log(true);
