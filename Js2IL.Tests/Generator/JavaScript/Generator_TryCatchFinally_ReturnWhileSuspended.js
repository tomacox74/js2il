"use strict";

function* g1() {
  try {
    console.log("g1:try");
    yield "T";
    console.log("g1:after-yield");
  } catch (e) {
    console.log("g1:catch:" + e);
    yield "C";
  } finally {
    console.log("g1:finally");
  }
  console.log("g1:after-try");
}

const it1 = g1();

let it1r1 = it1.next();
console.log("it1.r1.value=" + it1r1.value);
console.log("it1.r1.done=" + it1r1.done);

let it1r2 = it1.return("R1");
console.log("it1.r2.value=" + it1r2.value);
console.log("it1.r2.done=" + it1r2.done);

let it1r3 = it1.next();
console.log("it1.r3.value=" + it1r3.value);
console.log("it1.r3.done=" + it1r3.done);

function* g2() {
  try {
    console.log("g2:try");
    yield "T2";
    console.log("g2:after-yield");
  } catch (e) {
    console.log("g2:catch:" + e);
    yield "C2";
    console.log("g2:after-catch-yield");
  } finally {
    console.log("g2:finally");
  }
  console.log("g2:after-try");
}

const it2 = g2();

let it2r1 = it2.next();
console.log("it2.r1.value=" + it2r1.value);
console.log("it2.r1.done=" + it2r1.done);

let it2r2 = it2.throw("E2");
console.log("it2.r2.value=" + it2r2.value);
console.log("it2.r2.done=" + it2r2.done);

let it2r3 = it2.return("R2");
console.log("it2.r3.value=" + it2r3.value);
console.log("it2.r3.done=" + it2r3.done);

let it2r4 = it2.next();
console.log("it2.r4.value=" + it2r4.value);
console.log("it2.r4.done=" + it2r4.done);
