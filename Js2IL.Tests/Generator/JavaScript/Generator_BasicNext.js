"use strict";

function* g() {
  console.log("start");
  yield 1;
  const x = yield 2;
  console.log("x=" + x);
  return 3;
}

const it = g();
console.log("aftercall");

let r1 = it.next();
console.log(r1.value);
console.log(r1.done);

let r2 = it.next();
console.log(r2.value);
console.log(r2.done);

let r3 = it.next(42);
console.log(r3.value);
console.log(r3.done);

let r4 = it.next();
console.log(r4.value);
console.log(r4.done);
