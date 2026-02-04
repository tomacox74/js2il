"use strict";

// Ensure ECMA-262 global function properties can be used as first-class values.
// This mirrors common library patterns where parseInt/parseFloat/isFinite are passed around.

const p = parseInt;
const pf = parseFloat;
const fin = isFinite;

console.log(typeof p);
console.log(typeof pf);
console.log(typeof fin);

console.log(p === parseInt);
console.log(pf === parseFloat);
console.log(fin === isFinite);

// Smoke calls via value
console.log(p("15px", 10));
console.log(pf("1.25abc"));
console.log(fin(123));
