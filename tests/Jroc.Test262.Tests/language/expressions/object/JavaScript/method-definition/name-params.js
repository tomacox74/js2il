// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var value1 = {};
var value2 = {};
var value3 = {};
var arg1, arg2, arg3;
var obj = {
  method(a, b, c) {
    arg1 = a;
    arg2 = b;
    arg3 = c;
  }
};

obj.method(value1, value2, value3);

assert.sameValue(arg1, value1);
assert.sameValue(arg2, value2);
assert.sameValue(arg3, value3);
