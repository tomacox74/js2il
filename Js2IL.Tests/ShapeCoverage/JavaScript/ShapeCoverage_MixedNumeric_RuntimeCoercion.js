"use strict";

// Numbers flowing through an if/else join point then used in arithmetic.
// Exercises the coercion path when both branches store the same variable
// with numeric values and the join result is consumed by an operator.
// Expected: 10, 42
let x = 5;
let val;
if (x > 3) {
  val = x * 2; // 10
} else {
  val = 0;
}
console.log(val); // 10

let y = 21;
let out;
if (y > 100) {
  out = 0;
} else {
  out = y * 2; // 42
}
console.log(out); // 42
