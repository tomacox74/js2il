assert.sameValue(Number.isFinite("1"), false, "string");
assert.sameValue(Number.isFinite([1]), false, "[1]");
assert.sameValue(Number.isFinite(new Number(42)), false, "Number object");
assert.sameValue(Number.isFinite(false), false, "false");
assert.sameValue(Number.isFinite(true), false, "true");
assert.sameValue(Number.isFinite(undefined), false, "undefined");
assert.sameValue(Number.isFinite(null), false, "null");
assert.sameValue(Number.isFinite(Symbol("1")), false, "symbol");
assert.sameValue(Number.isFinite(), false, "no arg");
