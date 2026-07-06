// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var thisValue = null;
var method = {
  method() {
    'use strict';
    thisValue = this;
  }
}.method;

method();

assert.sameValue(thisValue, undefined);
