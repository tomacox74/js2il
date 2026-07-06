// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var global = (function() { return this; }());
var thisValue = null;
var method = {
  method() {
    thisValue = this;
  }
}.method;

method();

assert.sameValue(thisValue, global);
