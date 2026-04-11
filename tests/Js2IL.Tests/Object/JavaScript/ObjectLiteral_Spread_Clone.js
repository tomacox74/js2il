"use strict";

// Shallow clone via spread should not mutate the original.

const original = { a: 1 };
const clone = { ...original };

clone.a = 2;

console.log(original.a);
console.log(clone.a);
