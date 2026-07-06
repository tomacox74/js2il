var map = new Map();

map.set(0, undefined);
map.set(undefined, undefined);
map.set(false, undefined);
map.set(NaN, undefined);
map.set(null, undefined);
map.set('', undefined);
map.set(Symbol(), undefined);

assert.sameValue(map.size, 7, 'The value of `map.size` is `7`');

