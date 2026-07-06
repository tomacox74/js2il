var __string = new String("123");
var __re = /((1)|(12))((3)|(23))/;

assert.sameValue(
  __re.test(__string),
  __re.exec(__string) !== null,
  '__re.test("new String("123")") must return __re.exec(__string) !== null'
);
