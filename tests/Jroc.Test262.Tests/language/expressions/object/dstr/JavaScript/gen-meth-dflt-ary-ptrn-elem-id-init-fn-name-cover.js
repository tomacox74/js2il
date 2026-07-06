// Upstream: test/language/expressions/object/dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js
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
