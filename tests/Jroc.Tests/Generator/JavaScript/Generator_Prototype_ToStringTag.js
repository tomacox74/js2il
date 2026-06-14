"use strict";

function* g() {
  yield 1;
}

const gen = g();

// Object.prototype.toString.call(gen) must return "[object Generator]"
const tag = Object.prototype.toString.call(gen);
console.log(tag);

// Also works on a not-yet-started generator
const gen2 = g();
console.log(Object.prototype.toString.call(gen2));

// And on a completed generator
gen2.next();
gen2.next();
console.log(Object.prototype.toString.call(gen2));
