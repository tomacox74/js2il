// Upstream: test/language/function-code/10.4.3-1-35-s.js
"use strict";

(function () {
  assert.sameValue((function () {
    return typeof this;
  })(), "undefined");
  assert.sameValue(typeof this, "undefined");
})();
