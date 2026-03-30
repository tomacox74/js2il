"use strict";

const nested = [[1], [2]];
const copy = [...nested];

console.log(copy.length);
console.log(copy[0] === nested[0]);
console.log(copy[0][0]);
