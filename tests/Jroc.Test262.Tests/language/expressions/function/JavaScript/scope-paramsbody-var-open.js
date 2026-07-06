// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var x = 'outside';
var probeParams, probeBody;

(function(_ = probeParams = function() { return x; }) {
  var x = 'inside';
  probeBody = function() { return x; };
}());

assert.sameValue(probeParams(), 'outside');
assert.sameValue(probeBody(), 'inside');
