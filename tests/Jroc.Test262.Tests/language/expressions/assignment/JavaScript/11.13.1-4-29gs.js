// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 11.13.1-4-29gs
description: >
    Strict Mode - TypeError is thrown if the identifier 'Math.PI'
    appears as the LeftHandSideExpression of simple assignment(=)
flags: [onlyStrict]
---*/

"use strict";

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.throws(TypeError, function() {
  Math.PI = 20;
});
