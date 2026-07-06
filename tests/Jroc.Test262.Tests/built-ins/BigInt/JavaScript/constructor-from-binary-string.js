assert.sameValue(BigInt("0b1111"), 15n);
assert.sameValue(BigInt("0b10"), 2n);
assert.sameValue(BigInt("0b0"), 0n);
assert.sameValue(BigInt("0b1"), 1n);

let binaryString = "0b1";
for (let i = 0; i < 128; i++)
  binaryString += "0";

assert.sameValue(BigInt(binaryString), 340282366920938463463374607431768211456n);

assert.sameValue(BigInt("0B1111"), 15n);
assert.sameValue(BigInt("0B10"), 2n);
assert.sameValue(BigInt("0B0"), 0n);
assert.sameValue(BigInt("0B1"), 1n);

binaryString = "0B1";
for (let i = 0; i < 128; i++)
  binaryString += "0";

assert.sameValue(BigInt(binaryString), 340282366920938463463374607431768211456n);
