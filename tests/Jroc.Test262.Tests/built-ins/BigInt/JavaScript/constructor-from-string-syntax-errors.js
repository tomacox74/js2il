assert.throws(SyntaxError, function() {
  BigInt("10n");
});

assert.throws(SyntaxError, function() {
  BigInt("10x");
});

assert.throws(SyntaxError, function() {
  BigInt("10b");
});

assert.throws(SyntaxError, function() {
  BigInt("10.5");
});

assert.throws(SyntaxError, function() {
  BigInt("0b");
});

assert.throws(SyntaxError, function() {
  BigInt("-0x1");
});

assert.throws(SyntaxError, function() {
  BigInt("-0XFFab");
});

assert.throws(SyntaxError, function() {
  BigInt("0oa");
});

assert.throws(SyntaxError, function() {
  BigInt("000 12");
});

assert.throws(SyntaxError, function() {
  BigInt("0o");
});

assert.throws(SyntaxError, function() {
  BigInt("0x");
});

assert.throws(SyntaxError, function() {
  BigInt("00o");
});

assert.throws(SyntaxError, function() {
  BigInt("00b");
});

assert.throws(SyntaxError, function() {
  BigInt("00x");
});
