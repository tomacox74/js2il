// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var probe;

var C = class {
  // A parameter expression is necessary to trigger the creation of the scope
  // under test.
  static m(_ = null) {
    var x = 'inside';
    probe = function() { return x; };
  }
};
C.m();

var x = 'outside';

assert.sameValue(probe(), 'inside');
assert.sameValue(x, 'outside');
