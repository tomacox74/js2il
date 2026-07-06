// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var accessed = false;
var obj1 = {
  valueOf: function () {
    accessed = true;
    return 3;
  }
};
var obj2 = {
  toString: function () {
    return 4;
  }
};

assert.sameValue(obj1 > obj2, false, 'The result of (obj1 > obj2) is false');
assert.sameValue(accessed, true, 'The value of accessed is true');
