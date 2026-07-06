var s = new String("hello world");

assert.sameValue(s[NaN], undefined, 's[NaN]');
