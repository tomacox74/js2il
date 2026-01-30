"use strict";

let a = 1;
function f() {
  let a = 2;
  console.log(a);
}
f();
console.log(a);
