// Copyright (C) 2019 Alexey Shvayka. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
Object.defineProperty(Object.prototype, '__proto__', {
  set: function() {
    throw new Test262Error('should not be called');
  },
});

var A = class extends Array {};

assert.sameValue(new A(1).length, 1);
