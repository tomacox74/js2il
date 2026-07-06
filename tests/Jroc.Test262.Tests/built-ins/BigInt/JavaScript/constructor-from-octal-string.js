assert.sameValue(BigInt("0o7"), 7n);
assert.sameValue(BigInt("0o10"), 8n);
assert.sameValue(BigInt("0o20"), 16n);

assert.sameValue(BigInt("0O7"), 7n);
assert.sameValue(BigInt("0O10"), 8n);
assert.sameValue(BigInt("0O20"), 16n);
