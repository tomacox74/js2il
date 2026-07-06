var foo = {};
var map = new WeakMap([
  [foo, 1]
]);

assert.sameValue(map.set(foo, 1), map, '`map.set(foo, 1)` returns `map`');

