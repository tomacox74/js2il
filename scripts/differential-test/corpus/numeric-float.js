"use strict";
// Risk area: floating-point / boxed-vs-unboxed numeric materialisation

// Division producing float
console.log(7 / 2);             // 3.5
console.log(Math.floor(7 / 2)); // 3
console.log(Math.ceil(7 / 2));  // 4

// Float accumulation in a loop (var to avoid known for-let compound-assign bug)
var acc = 0;
for (var i = 1; i <= 5; i++) {
    acc += i * 0.5;
}
console.log(acc); // 7.5

// Integer that becomes float mid-computation
var x = 4;
x = x * 1.25;
console.log(x); // 5

// Comparison after float conversion
const result = x > 4 ? "above-four" : "four-or-less";
console.log(result); // above-four

// Special float values
console.log(0 / 0);   // NaN
console.log(1 / 0);   // Infinity
console.log(-1 / 0);  // -Infinity

// parseInt / parseFloat
console.log(parseInt("42px"));    // 42
console.log(parseFloat("3.14s")); // 3.14
console.log(Number("100"));       // 100
