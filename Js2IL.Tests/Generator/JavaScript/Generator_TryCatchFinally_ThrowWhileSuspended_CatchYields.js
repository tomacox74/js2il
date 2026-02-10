"use strict";

function* g() {
  try {
    console.log("try");
    yield 1;
    console.log("after-yield");
  } catch (e) {
    console.log("catch=" + e);
    yield 2;
    console.log("after-catch-yield");
  } finally {
    console.log("finally");
  }

  console.log("after-try");
  return 3;
}

const it = g();

let r1 = it.next();
console.log("r1.value=" + r1.value);
console.log("r1.done=" + r1.done);

let r2 = it.throw("boom");
console.log("r2.value=" + r2.value);
console.log("r2.done=" + r2.done);

let r3 = it.next();
console.log("r3.value=" + r3.value);
console.log("r3.done=" + r3.done);

let r4 = it.next();
console.log("r4.value=" + r4.value);
console.log("r4.done=" + r4.done);
