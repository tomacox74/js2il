var map = new WeakMap();
var foo = {};
var bar = {};
var baz = {};

map.set(foo, 1);
map.set(bar, 2);
map.set(baz, 3);

assert(map.has(foo));
assert(map.has(bar));
assert(map.has(baz));

