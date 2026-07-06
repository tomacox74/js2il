// Copyright (C) 2015 André Bargull. All rights reserved.
// Copyright (C) 2024 Sony Interactive Entertainment Inc. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
function DummyError() { }

assert.throws(DummyError, function() {
  var base = {};
  var prop = function() {
    throw new DummyError();
  };
  var expr = function() {
    throw new Test262Error("right-hand side expression evaluated");
  };

  base[prop()] = expr();
});

assert.throws(DummyError, function() {
  var base = {};
  var prop = {
    toString: function() {
      throw new Test262Error("property key evaluated");
    }
  };
  var expr = function() {
    throw new DummyError();
  };

  base[prop] = expr();
});
