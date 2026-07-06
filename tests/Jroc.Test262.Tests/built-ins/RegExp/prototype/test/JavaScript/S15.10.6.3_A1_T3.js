var __string = new Object("abcdefghi");
var __re = /a[a-z]{2,4}/;

assert.sameValue(
  __re.test(__string),
  __re.exec(__string) !== null,
  '__re.test("new Object("abcdefghi")") must return __re.exec(__string) !== null'
);
