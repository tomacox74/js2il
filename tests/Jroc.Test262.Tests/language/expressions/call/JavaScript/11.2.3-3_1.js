// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var fooCalled = false;
    function foo(){ fooCalled = true; } 
    
    var o = { }; 
assert.throws(TypeError, function() {
        o.bar( foo() );
        throw new Test262Error("o.bar does not exist!");
});
assert.sameValue(fooCalled, true, 'fooCalled');
