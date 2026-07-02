// Upstream: test/language/expressions/function/dstr/obj-ptrn-id-init-fn-name-gen.js
function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var callCount = 0;
var f;
f = function({ gen = function* () {}, xGen = function* x() {} }) {
  assert.sameValue(gen.name, 'gen');
  assert.notSameValue(xGen.name, 'xGen');
  callCount = callCount + 1;
};

f({});
assert.sameValue(callCount, 1);
