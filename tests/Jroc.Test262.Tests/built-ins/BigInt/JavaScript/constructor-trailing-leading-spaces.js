assert.sameValue(BigInt("   0b1111"), 15n);
assert.sameValue(BigInt("18446744073709551616   "), 18446744073709551616n);
assert.sameValue(BigInt("   7   "), 7n);
assert.sameValue(BigInt("   -197   "), -197n);
assert.sameValue(BigInt("     "), 0n);
