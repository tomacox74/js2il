"use strict";

function key() {
  return "a";
}

let obj = { a: 1 };

// Forces LIRGetItem + LIRSetItem results to be inlined into call args,
// with non-trivial index/value expressions (maxstack regression).
console.log(obj[key()]);
console.log(obj[key()] = (1 + 2));
console.log(obj.a);
