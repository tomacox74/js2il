var s = new Set();

assert.sameValue(s.size, 0, "The value of `s.size` is `0`");

s.add(0);

assert.sameValue(s.size, 1, "The value of `s.size` is `1`, after executing `s.add(0)`");

s.delete(0);

assert.sameValue(s.size, 0, "The value of `s.size` is `0`, after executing `s.delete(0)`");

