var s = String("hello world");

assert.sameValue(s[Infinity], undefined, 's[Infinity]');
