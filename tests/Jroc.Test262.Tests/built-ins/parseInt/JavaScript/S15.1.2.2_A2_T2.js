assert.sameValue(parseInt("\u00201"), parseInt("1"), 'parseInt("\\u00201") must return the same value returned by parseInt("1")');
assert.sameValue(parseInt("\u0020\u0020-1"), parseInt("-1"), 'parseInt("\\u0020\\u0020-1") must return the same value returned by parseInt("-1")');
assert.sameValue(parseInt(" 1"), parseInt("1"), 'parseInt(" 1") must return the same value returned by parseInt("1")');
assert.sameValue(parseInt("       1"), parseInt("1"), 'parseInt(" 1") must return the same value returned by parseInt("1")');

assert.sameValue(
  parseInt("       \u0020       \u0020-1"),
  parseInt("-1"),
  'parseInt(" \\u0020 \\u0020-1") must return the same value returned by parseInt("-1")'
);

//CHECK#6
assert.sameValue(parseInt("\u0020"), NaN, 'parseInt("\\u0020") must return NaN');
