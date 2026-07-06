// Copyright (C) 2020 Rick Waldron. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var count = 0;
var global = this;

Object.defineProperty(this, "x", {
  configurable: true,
  value: 1
});

(function() {
  "use strict";
  assert.throws(ReferenceError, () => {
    count++;
    x = (delete global.x, 2);
    count++;
  });
  count++;
})();

assert.sameValue(count, 2);
assert(!('x' in this));
assert(!('x' in global));
