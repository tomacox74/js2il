// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x = 'outside';
var probe1, probe2;

(function(
    _ = probe1 = function() { return x; },
    ...[__ = (eval('var x = "inside";'), probe2 = function() { return x; })]
  ) {
}());

assert.sameValue(probe1(), 'inside');
assert.sameValue(probe2(), 'inside');
