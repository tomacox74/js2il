var non_w = "\f\n\r\t\v~`!@#$%^&*()-+={[}]|\\:;'<,>./? " + '"';

assert.sameValue(
  /\w/.exec(non_w),
  null,
  '/w/.exec(""fnrtv~`!@#$%^&*()-+={[}]|:;\'<,>./? " + \'"\'") must return null'
);

var non_W = "_0123456789_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
var regexp_w = /\w/g;
var k = 0;
while (regexp_w.exec(non_W) !== null) {
   k++;
}

assert.sameValue(non_W.length, k, 'The value of non_W.length is expected to equal the value of k');
