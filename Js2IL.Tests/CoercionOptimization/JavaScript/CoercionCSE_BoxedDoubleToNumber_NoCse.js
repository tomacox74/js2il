"use strict";

// This verifies the non-optimized path: repeated Number() calls on a scope
// variable that is represented as boxed/object should NOT be CSE'd by
// LIRCoercionCSE, because only unboxed primitive sources are eligible.

function compute(n) {
    let x = n - 0; // coerce to double
    let a = Number(x);
    let b = Number(x);
    return a + b;
}

console.log(compute(5));    // 10
console.log(compute(3.14)); // 6.28
console.log(compute(-2));   // -4
