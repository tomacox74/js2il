"use strict";

// Tests that repeated Number() calls on the same typed-double variable are
// CSE'd (the second call is replaced by a copy of the first result) while
// still producing correct output.

function compute(n) {
    let x = n - 0; // coerce to double
    let a = Number(x);
    let b = Number(x);
    return a + b;
}

console.log(compute(5));    // 10
console.log(compute(3.14)); // 6.28
console.log(compute(-2));   // -4
