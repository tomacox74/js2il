// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: The head's bound names may include "let"
esid: sec-for-in-and-for-of-statements-static-semantics-early-errors
es6id: 13.7.5
flags: [noStrict]
---*/
// test262 execution-port helpers
var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function (actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function (actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.throws = function (expectedError, fn) {
  var passed = false;
  try {
    fn();
  } catch (error) {
    passed = error instanceof expectedError;
  }
  console.log(passed);
};


var iterCount = 0;

for (var let of [23]) {
  assert.sameValue(let, 23);
  iterCount += 1;
}

assert.sameValue(iterCount, 1);
