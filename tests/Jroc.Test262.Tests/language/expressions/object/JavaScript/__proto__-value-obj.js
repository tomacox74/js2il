// Copyright (C) 2015 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var proto = {};

var object = {
  __proto__: proto
};

assert.sameValue(Object.getPrototypeOf(object), proto);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'), undefined
);
