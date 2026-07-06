var map = new Map();

assert.sameValue(map.size, 0, 'The value of `map.size` is `0`');

map.set(1, 1);
assert.sameValue(
  map.size, 1,
  'The value of `map.size` is `1`, after executing `map.set(1, 1)`'
);

map.delete(1);
assert.sameValue(
  map.size, 0,
  'The value of `map.size` is `0`, after executing `map.delete(1)`'
);

