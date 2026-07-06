// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
function F() {
  this.af = _ => {
    return this;
  };
}

var usurper = {};
var f = new F();

assert.sameValue(f.af(), f);
assert.sameValue(f.af.apply(usurper), f);
assert.sameValue(f.af.call(usurper), f);
assert.sameValue(f.af.bind(usurper)(), f);
