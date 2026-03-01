"use strict";
// Risk area: logical && and || – value semantics and short-circuit

// Boolean results
const t = true, f = false;
console.log(t && t); // true
console.log(t && f); // false
console.log(f || t); // true
console.log(f || f); // false

// Value (non-boolean) semantics
const zero = 0, nonzero = 42, str = "hello";
console.log(zero || nonzero);   // 42
console.log(nonzero && str);    // hello
console.log(zero && str);       // 0
console.log(null ?? "default"); // default
console.log(0 ?? "default");    // 0  (nullish only for null/undefined)

// Short-circuit: right-hand side must NOT execute
let sideEffect = 0;
false && (sideEffect = 1);
true  || (sideEffect = 2);
console.log(sideEffect); // 0

// Chained || for fallback
function first(a, b, c) { return a || b || c; }
console.log(first(0, null, "found")); // found
console.log(first(1, 2, 3));          // 1
