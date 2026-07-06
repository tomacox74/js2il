// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var callCount = 0;
(function f(n) {
  if (n === 0) {
    callCount += 1
    return;
  }
  return false || f(n - 1);
}($MAX_ITERATIONS));
assert.sameValue(callCount, 1);
