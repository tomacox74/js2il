assert.sameValue(BigInt("10"), 10n);
assert.sameValue(BigInt("18446744073709551616"), 18446744073709551616n);
assert.sameValue(BigInt("7"), 7n);
assert.sameValue(BigInt("88"), 88n);
assert.sameValue(BigInt("900"), 900n);

assert.sameValue(BigInt("-10"), -10n);
assert.sameValue(BigInt("-18446744073709551616"), -18446744073709551616n);
assert.sameValue(BigInt("-7"), -7n);
assert.sameValue(BigInt("-88"), -88n);
assert.sameValue(BigInt("-900"), -900n);
