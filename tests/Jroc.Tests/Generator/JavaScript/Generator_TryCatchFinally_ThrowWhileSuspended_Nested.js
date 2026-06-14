"use strict";

function* g() {
  try {
    console.log("outer-try");

    try {
      console.log("inner-try");
      yield 1;
      console.log("inner-after");
    } catch (e) {
      console.log("inner-catch=" + e);
    } finally {
      console.log("inner-finally");
    }

    console.log("outer-after-inner");
    yield 2;
  } finally {
    console.log("outer-finally");
  }

  console.log("after-all");
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
