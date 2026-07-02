// Upstream: test/language/function-code/10.4.3-1-35-s.js
"use strict";

function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };

(function () {
  assert.sameValue((function () {
    return typeof this;
  })(), "undefined");
  assert.sameValue(typeof this, "undefined");
})();
