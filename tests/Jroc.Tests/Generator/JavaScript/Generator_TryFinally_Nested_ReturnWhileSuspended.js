"use strict";

function* g() {
  try {
    console.log("outer-try");
    yield 1;

    try {
      console.log("inner-try");
      yield 2;
      console.log("inner-after-yield");
    } finally {
      console.log("inner-finally");
    }

    console.log("outer-after-inner");
  } finally {
    console.log("outer-finally");
  }
}

const it = g();

let r1 = it.next();
console.log("r1.value=" + r1.value);
console.log("r1.done=" + r1.done);

let r2 = it.next();
console.log("r2.value=" + r2.value);
console.log("r2.done=" + r2.done);

let r3 = it.return(99);
console.log("r3.value=" + r3.value);
console.log("r3.done=" + r3.done);

let r4 = it.next();
console.log("r4.value=" + r4.value);
console.log("r4.done=" + r4.done);
