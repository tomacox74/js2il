"use strict";

// ?:  result feeding arithmetic - join materialization tripwire
// Both branches of ternary produce a number; the result must be
// materialized to a slot before the downstream arithmetic uses it.
// Expected: 15 then 25
let flag = true;
let pick = flag ? 10 : 20;
let result = pick + 5;
console.log(result); // 15

flag = false;
pick = flag ? 10 : 20;
result = pick + 5;
console.log(result); // 25
