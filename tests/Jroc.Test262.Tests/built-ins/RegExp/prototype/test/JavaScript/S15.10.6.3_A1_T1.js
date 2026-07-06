var __string = "123";
var __re = /1|12/;

assert.sameValue(
  __re.test(__string),
  __re.exec(__string) !== null,
  '__re.test(""123"") must return __re.exec(__string) !== null'
);
