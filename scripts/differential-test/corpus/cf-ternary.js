"use strict";
// Risk area: control-flow joins via ternary (?:)

// Basic ternary
const a = 7, b = 3;
const max = a > b ? a : b;
console.log(max); // 7

// Nested ternary (phi-like merge)
const x = 5;
const label = x < 0 ? "neg" : x === 0 ? "zero" : "pos";
console.log(label); // pos

// Ternary inside a loop accumulator
let sum = 0;
for (let i = 0; i < 6; i++) {
    sum += (i % 2 === 0) ? i : -i;
}
console.log(sum); // 0-1+2-3+4-5 = -3

// Ternary branch that itself is a ternary
const n = 42;
const bucket = n < 10 ? "small" : n < 100 ? "medium" : "large";
console.log(bucket); // medium

// Ternary with function call value
function double(v) { return v * 2; }
const y = 8;
const r = y > 5 ? double(y) : y + 1;
console.log(r); // 16
