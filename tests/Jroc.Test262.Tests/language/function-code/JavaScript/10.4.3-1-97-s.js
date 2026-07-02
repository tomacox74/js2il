// Upstream: test/language/function-code/10.4.3-1-97-s.js
function assert(value) { console.log(!!value); }

var global = this;
function f() { return this === global; }
assert((function () { "use strict"; return f.bind(undefined)(); })());
