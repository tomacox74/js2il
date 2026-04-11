"use strict";

// && result feeding arithmetic - join materialization tripwire
// Short-circuit join must be pinned before downstream arithmetic reads it.
// Expected: 13 then 10
const a = 1;
const b = 3;
const x = a && b; // truthy: x = 3
const result1 = x + 10;
console.log(result1); // 13

const c = 0;
const y = c && b; // falsy short-circuits: y = 0
const result2 = y + 10;
console.log(result2); // 10
