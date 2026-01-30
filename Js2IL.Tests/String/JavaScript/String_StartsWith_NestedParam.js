"use strict";\r\n\r\nfunction outer(s) {
  function inner(p) { return s.startsWith(p); }
  return inner('a');
}
console.log(outer('abc'));
