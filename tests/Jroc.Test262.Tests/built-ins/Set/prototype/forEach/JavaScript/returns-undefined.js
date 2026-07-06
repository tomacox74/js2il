var s = new Set([1]);

assert.sameValue(
  s.forEach(function() {}),
  undefined,
  "`s.forEach(function() {})` returns `undefined`"
);

