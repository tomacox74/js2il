var s = new WeakSet();

assert.sameValue(s.delete(1), false);
assert.sameValue(s.delete(''), false);
assert.sameValue(s.delete(null), false);
assert.sameValue(s.delete(undefined), false);
assert.sameValue(s.delete(true), false);
assert.sameValue(s.delete(Symbol.for('registered symbol')), false, 'Registered symbol not allowed as value');

