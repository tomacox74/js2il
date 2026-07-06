// Copyright (C) 2024 Sony Interactive Entertainment Inc. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.throws(TypeError, function() {
  var base = null;
  var prop = {
    toString: function() {
      throw new Test262Error("property key evaluated");
    }
  };

  base[prop];
});

assert.throws(TypeError, function() {
  var base = undefined;
  var prop = {
    toString: function() {
      throw new Test262Error("property key evaluated");
    }
  };

  base[prop];
});
