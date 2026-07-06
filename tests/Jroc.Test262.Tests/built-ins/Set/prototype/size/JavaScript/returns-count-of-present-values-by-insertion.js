var s = new Set();

s.add(0);
s.add(undefined);
s.add(false);
s.add(NaN);
s.add(null);
s.add("");
s.add(Symbol());

assert.sameValue(s.size, 7, "The value of `s.size` is `7`");

