"use strict";

// Validate min/max behavior with NaN and empty args
const a = Math.min(3, 2, 5);
const b = Math.max(3, 2, 5);
const c = Math.min();
const d = Math.max();
const e = Math.min(1, NaN, 2);
const f = Math.max(1, NaN, 2);
console.log(a, b);
console.log(c, d);
console.log(e, f);
