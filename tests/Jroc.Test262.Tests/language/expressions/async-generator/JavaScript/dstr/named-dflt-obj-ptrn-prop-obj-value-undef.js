// Upstream: test/language/expressions/async-generator/dstr/named-dflt-obj-ptrn-prop-obj-value-undef.js
function assert(value) { console.log(!!value); }
assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor || error.constructor === expectedCtor || error.name === expectedCtor.name);
  }
};

var f;
f = async function* h({ w: { x, y, z } = undefined } = { }) {};

assert.throws(TypeError, function() {
  f();
});
