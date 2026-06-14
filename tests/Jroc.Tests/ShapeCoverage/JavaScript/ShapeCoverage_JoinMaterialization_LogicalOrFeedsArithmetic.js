"use strict";

// || result feeding arithmetic - join materialization tripwire
// Short-circuit join must be pinned before downstream arithmetic reads it.
// Expected: 15 then 13
const a = 0;
const b = 5;
const x = a || b; // falsy falls through: x = 5
const result1 = x + 10;
console.log(result1); // 15

const c = 3;
const y = c || b; // truthy short-circuits: y = 3
const result2 = y + 10;
console.log(result2); // 13
