// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var y = {};
var retVal;

y[Symbol.toPrimitive] = function() {
  return retVal;
};

retVal = 86;
assert.sameValue(0 == y, false, 'number primitive (not equal)');
assert.sameValue(86 == y, true, 'number primitive (equal)');

retVal = 'str';
assert.sameValue(0 == y, false, 'string primitive (not equal)');
assert.sameValue('str' == y, true, 'sting primitive (equal)');

retVal = Symbol.toPrimitive;
assert.sameValue(0 == y, false, 'symbol (not equal)');
assert.sameValue(Symbol.toPrimitive == y, true, 'symbol (equal)');
