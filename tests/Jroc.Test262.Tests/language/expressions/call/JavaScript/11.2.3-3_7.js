// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var o = { }; 
    Object.defineProperty(o, "bar", {get: function()  {this.barGetter = true; return 42;}, 
                                     set: function(x) {this.barSetter = true; }});
assert.throws(TypeError, function() {
        o.foo( o["bar"] );
        throw new Test262Error("o.foo does not exist!");
});
assert.sameValue(o.barGetter, true, 'o.barGetter');
assert.sameValue(o.barSetter, undefined, 'o.barSetter');
