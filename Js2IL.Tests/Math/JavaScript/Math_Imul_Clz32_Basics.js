"use strict";\r\n\r\n// Validate imul 32-bit wrapping and clz32 counts
const a = Math.imul(2, 3);
const b = Math.imul((0 - 1), 2);
const c = Math.imul((0 - 1), 0); // should be -0
const d = Math.clz32(0);
const e = Math.clz32(1);
const f = Math.clz32(0xF0);
function toStr(n){
  return (n === 0 && (1/n) < 0) ? "-0" : ("" + n);
}
console.log(a + " " + b + " " + toStr(c));
console.log(d, e, f);
