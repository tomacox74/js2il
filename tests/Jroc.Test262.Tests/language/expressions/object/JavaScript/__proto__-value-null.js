// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var object = {
  __proto__: null
};

assert.sameValue(Object.getPrototypeOf(object), null);
assert.sameValue(
  Object.getOwnPropertyDescriptor(object, '__proto__'), undefined
);
