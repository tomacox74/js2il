var s = new String("hello world");
s.foo = 1;


assert.sameValue(s["foo"], 1, 's["foo"]');
