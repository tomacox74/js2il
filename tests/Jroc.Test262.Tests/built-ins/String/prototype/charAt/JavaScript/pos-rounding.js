assert.sameValue('abc'.charAt(-0.99999), 'a', '-0.99999');
assert.sameValue('abc'.charAt(-0.00001), 'a', '-0.00001');
assert.sameValue('abc'.charAt(0.00001), 'a', '0.00001');
assert.sameValue('abc'.charAt(0.99999), 'a', '0.99999');
assert.sameValue('abc'.charAt(1.00001), 'b', '1.00001');
assert.sameValue('abc'.charAt(1.99999), 'b', '1.99999');
