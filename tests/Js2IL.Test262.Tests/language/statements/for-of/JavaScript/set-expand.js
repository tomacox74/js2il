// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
description: Set entry insertaion during traversal using for..of
info: |
    New entries inserted into a Set instance during traversal should be
    visited.
es6id: 13.6.4
features: [Set]
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


var set = new Set();
var iterationCount = 0;

var first = 0;
var second = 1;

set.add(0);

for (var x of set) {
  assert.sameValue(x, first);

  first = second;
  second = null;

  set.add(1);

  iterationCount += 1;
}

assert.sameValue(iterationCount, 2);
