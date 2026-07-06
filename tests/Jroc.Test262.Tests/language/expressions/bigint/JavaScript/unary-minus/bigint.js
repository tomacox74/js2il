assert.sameValue(-0n, 0n, "-0n === 0n");
assert.sameValue(-(0n), 0n, "-(0n) === 0n");
assert.notSameValue(-1n, 1n, "-1n !== 1n");
assert.sameValue(-(1n), -1n, "-(1n) === -1n");
assert.notSameValue(-(1n), 1n, "-(1n) !== 1n");
assert.sameValue(-(-1n), 1n, "-(-1n) === 1n");
assert.notSameValue(-(-1n), -1n, "-(-1n) !== -1n");
assert.sameValue(- - 1n, 1n, "- - 1n === 1n");
assert.notSameValue(- - 1n, -1n, "- - 1n !== -1n");
assert.sameValue(
  -(0x1fffffffffffff01n), -0x1fffffffffffff01n,
  "-(0x1fffffffffffff01n) === -0x1fffffffffffff01n");
assert.notSameValue(
  -(0x1fffffffffffff01n), 0x1fffffffffffff01n,
  "-(0x1fffffffffffff01n) !== 0x1fffffffffffff01n");
assert.notSameValue(
  -(0x1fffffffffffff01n), -0x1fffffffffffff00n,
  "-(0x1fffffffffffff01n) !== -0x1fffffffffffff00n");
