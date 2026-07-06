// Copyright 2015 Microsoft Corporation. All rights reserved.
// This code is governed by the license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var a;
function foo(){
    eval("a = 10");
    return ()=>a;
 }

assert.sameValue(foo()(), 10, "Closure variable was captured incorrectly.");
