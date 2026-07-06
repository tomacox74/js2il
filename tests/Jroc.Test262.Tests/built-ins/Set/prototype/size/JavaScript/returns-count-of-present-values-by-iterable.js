var s = new Set([0, undefined, false, NaN, null, "", Symbol()]);

assert.sameValue(s.size, 7, "The value of `s.size` is `7`");

