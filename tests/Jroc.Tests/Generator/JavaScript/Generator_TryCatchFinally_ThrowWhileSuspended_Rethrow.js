"use strict";

function* g() {
  try {
    console.log("try");
    yield 1;
  } catch (e) {
    console.log("catch=" + e);
    throw e;
  } finally {
    console.log("finally");
  }
}

const it = g();

let r1 = it.next();
console.log("r1.value=" + r1.value);
console.log("r1.done=" + r1.done);

try {
  it.throw("boom");
  console.log("throw-returned");
} catch (e) {
  console.log("caught=" + e);
}

let r2 = it.next();
console.log("r2.value=" + r2.value);
console.log("r2.done=" + r2.done);
