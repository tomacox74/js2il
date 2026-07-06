assert.throws(RangeError, function() {
  BigInt(0.00005);
});

assert.throws(RangeError, function() {
  BigInt(-0.00005);
});

assert.throws(RangeError, function() {
  BigInt(.1);
});

assert.throws(RangeError, function() {
  BigInt(-.1);
});

assert.throws(RangeError, function() {
  BigInt(1.1);
});

assert.throws(RangeError, function() {
  BigInt(-1.1);
});

assert.throws(RangeError, function() {
  BigInt(Number.MIN_VALUE);
});
