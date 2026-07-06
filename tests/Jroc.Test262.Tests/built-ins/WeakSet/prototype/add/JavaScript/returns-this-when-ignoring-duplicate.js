var foo = {};
var s = new WeakSet([foo]);

assert.sameValue(s.add(foo), s, '`s.add(foo)` returns `s`');

