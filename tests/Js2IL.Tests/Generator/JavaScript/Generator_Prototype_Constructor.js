"use strict";

function* g() {
  yield 1;
}

const gen = g();
const gen2 = g();

// gen.constructor must be a function
const ctor = gen.constructor;
console.log(typeof ctor);

// gen.constructor must be stable (same reference for all instances)
console.log(gen.constructor === gen2.constructor);
