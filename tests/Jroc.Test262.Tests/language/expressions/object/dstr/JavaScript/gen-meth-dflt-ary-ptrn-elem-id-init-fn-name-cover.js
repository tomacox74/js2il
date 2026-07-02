// Upstream: test/language/expressions/object/dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js
function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var callCount = 0;
var obj = {
  *method([cover = (function () {}), xCover = (0, function() {})] = []) {
    assert.sameValue(cover.name, 'cover');
    assert.notSameValue(xCover.name, 'xCover');
    callCount = callCount + 1;
  }
};

obj.method().next();
assert.sameValue(callCount, 1);
