// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.sameValue(typeof (() => {}), "function");
assert.sameValue(Object.getPrototypeOf(() => {}), Function.prototype);
assert.sameValue("prototype" in (() => {}), false);
