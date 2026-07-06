assert.sameValue(Number.isNaN("NaN"), false, "string");
assert.sameValue(Number.isNaN([NaN]), false, "[NaN]");
assert.sameValue(Number.isNaN(new Number(NaN)), false, "Number object");
assert.sameValue(Number.isNaN(false), false, "false");
assert.sameValue(Number.isNaN(true), false, "true");
assert.sameValue(Number.isNaN(undefined), false, "undefined");
assert.sameValue(Number.isNaN(null), false, "null");
assert.sameValue(Number.isNaN(Symbol("1")), false, "symbol");
assert.sameValue(Number.isNaN(), false, "no arg");
