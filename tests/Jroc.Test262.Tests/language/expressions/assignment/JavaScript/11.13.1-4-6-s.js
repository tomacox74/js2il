// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es5id: 11.13.1-4-6-s
description: >
    simple assignment throws TypeError if LeftHandSide is a readonly
    property in strict mode (Function.length)
flags: [onlyStrict]
---*/

"use strict";

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
assert.throws(TypeError, function() {
    Function.length = 42;
});
