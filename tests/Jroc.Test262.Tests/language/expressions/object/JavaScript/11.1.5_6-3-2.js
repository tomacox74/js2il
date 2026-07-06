// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var o = {get foo(){return 1;}};
  var desc = Object.getOwnPropertyDescriptor(o,"foo");

assert.sameValue(desc.set, undefined, 'desc.set');
