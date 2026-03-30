"use strict";

const source = new Int32Array([11, 22, 33, 44, 55]);
const sliced = source.slice(1, 4);

console.log(sliced.length);
for (let i = 0; i < sliced.length; i++) {
    console.log(sliced[i]);
}

sliced[0] = 99;
console.log(source[1]);

const full = source.slice();
console.log(full.length);
console.log(full[4]);
