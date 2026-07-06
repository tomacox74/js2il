var s = new Set();

s.add(NaN)

assert.sameValue(s.has(NaN), true, "`s.has(NaN)` returns `true`");

