var s = new String("hello world");

assert.sameValue(s[Math.pow(2, 32) - 1], undefined, 's[Math.pow(2, 32)-1]');
