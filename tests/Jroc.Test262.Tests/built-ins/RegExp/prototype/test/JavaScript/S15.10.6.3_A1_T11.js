var __string = new Number(1.012);
var __re = /2|12/;

assert.sameValue(
  __re.test(__string),
  __re.exec(__string) !== null,
  '__re.test(new Number(1.012)) must return __re.exec(__string) !== null'
);
