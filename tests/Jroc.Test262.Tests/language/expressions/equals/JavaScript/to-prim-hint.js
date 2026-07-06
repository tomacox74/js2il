// Copyright (C) 2017 Robin Templeton. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
let count = 0;
let obj = {
  [Symbol.toPrimitive](hint) {
    count += 1;
    assert.sameValue(hint, "default");
    return 1;
  }
};

assert.sameValue(true == obj, true);
assert.sameValue(count, 1);
assert.sameValue(obj == true, true);
assert.sameValue(count, 2);
