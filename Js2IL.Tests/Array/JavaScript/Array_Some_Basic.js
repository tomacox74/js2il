"use strict";\r\n\r\n// ECMA-262: https://tc39.es/ecma262/#sec-array.prototype.some

const arr = [1, 2, 3];
console.log(arr.some(x => x === 2));
console.log(arr.some(x => x === 4));

let calls = 0;
console.log(arr.some(x => { calls++; return x === 2; }));
console.log(calls);

console.log([].some(x => true));
