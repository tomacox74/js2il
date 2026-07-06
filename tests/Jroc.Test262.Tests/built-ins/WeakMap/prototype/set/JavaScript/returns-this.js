var map = new WeakMap();

assert.sameValue(map.set({}, 1), map, '`map.set({}, 1)` returns `map`');

