"use strict";

// Boxed numbers from a function return path used in arithmetic.
// The return value is typed as object - exercises unboxing before each operator.
// Expected: 7, -1, 12, 0.75, 3
function id(n) { return n; }

const x = id(3);
const y = id(4);
console.log(x + y); // 7
console.log(x - y); // -1
console.log(x * y); // 12
console.log(x / y); // 0.75
console.log(x % y); // 3
