"use strict";

let called = 0;
function key() {
  called++;
  return "x";
}

const o = null;
console.log(o?.[key()]);
console.log(called);
