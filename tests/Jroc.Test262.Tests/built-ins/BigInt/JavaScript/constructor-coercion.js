var first = true;
var v = {
  [Symbol.toPrimitive]: function() {
    if (first) {
      first = false;
      return "42";
    }
    throw new Test262Error("Symbol.toPrimitive should only be invoked once");
  },
};

assert.sameValue(BigInt(v), 42n, "BigInt constructor should use the post-ToPrimitive value as the argument to ToBigInt");
