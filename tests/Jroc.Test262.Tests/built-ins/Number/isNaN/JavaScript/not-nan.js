assert.sameValue(Number.isNaN(0), false, "0");
assert.sameValue(Number.isNaN(-0), false, "-0");
assert.sameValue(Number.isNaN(1), false, "1");
assert.sameValue(Number.isNaN(-1), false, "-1");
assert.sameValue(Number.isNaN(1.1), false, "1.1");
assert.sameValue(Number.isNaN(1e10), false, "1e10");
assert.sameValue(Number.isNaN(Infinity), false, "Infinity");
assert.sameValue(Number.isNaN(-Infinity), false, "-Infinity");
