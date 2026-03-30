"use strict";

const a = [1, 2];
const b = [3, 4];
const c = [5];

console.log([...a, ...b].join(","));
console.log([...a, ...b, ...c].join(","));
