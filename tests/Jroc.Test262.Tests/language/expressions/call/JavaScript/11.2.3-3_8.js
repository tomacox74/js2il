// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var fooCalled = false;
    function foo(){ fooCalled = true; } 
assert.throws(TypeError, function() {
        this.bar( foo() );
        throw new Test262Error("this.bar does not exist!");
});
assert.sameValue(fooCalled, true, 'fooCalled');
