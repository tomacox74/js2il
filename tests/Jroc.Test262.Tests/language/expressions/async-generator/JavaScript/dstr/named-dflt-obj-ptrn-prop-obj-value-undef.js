// Upstream: test/language/expressions/async-generator/dstr/named-dflt-obj-ptrn-prop-obj-value-undef.js
var f;
f = async function* h({ w: { x, y, z } = undefined } = { }) {};

assert.throws(TypeError, function() {
  f();
});
