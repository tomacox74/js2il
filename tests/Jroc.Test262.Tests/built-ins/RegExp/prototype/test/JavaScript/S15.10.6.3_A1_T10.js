var __string = 1.01;
var __re = /1|12/;

assert.sameValue(
  __re.test(__string),
  __re.exec(__string) !== null,
  '__re.test(1.01) must return __re.exec(__string) !== null'
);
