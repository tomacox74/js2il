var map = new Map();

assert.sameValue(map.size, 0, 'The value of `map.size` is `0`');

map.set(1, 1);
map.set(2, 2);
assert.sameValue(
  map.size, 2,
  'The value of `map.size` is `2`'
);

map.clear();
assert.sameValue(
  map.size, 0,
  'The value of `map.size` is `0`, after executing `map.clear()`'
);

