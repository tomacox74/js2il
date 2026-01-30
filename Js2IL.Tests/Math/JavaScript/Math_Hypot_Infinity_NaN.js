"use strict";

// Validate hypot with Infinity, NaN and basic Pythagoras
const a = Math.hypot(3, 4);
const b = Math.hypot(Infinity, 1);
const c = Math.hypot(NaN, 1);
console.log(a);
console.log(b);
console.log(c);
