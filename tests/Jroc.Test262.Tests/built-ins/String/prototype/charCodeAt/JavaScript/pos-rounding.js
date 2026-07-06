var aCode = 97;
var bCode = 98;

assert.sameValue('abc'.charCodeAt(-0.99999), aCode, '-0.99999');
assert.sameValue('abc'.charCodeAt(-0.00001), aCode, '-0.00001');
assert.sameValue('abc'.charCodeAt(0.00001), aCode, '0.00001');
assert.sameValue('abc'.charCodeAt(0.99999), aCode, '0.99999');
assert.sameValue('abc'.charCodeAt(1.00001), bCode, '1.00001');
assert.sameValue('abc'.charCodeAt(1.99999), bCode, '1.99999');
