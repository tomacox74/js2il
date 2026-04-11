"use strict";

const source = new Int32Array([10, 20, 30, 40, 50]);

const negative = source.slice(-3, -1);
console.log(negative.length);
for (let i = 0; i < negative.length; i++) {
    console.log(negative[i]);
}

const clamped = source.slice(-20, 2);
console.log(clamped.length);
for (let i = 0; i < clamped.length; i++) {
    console.log(clamped[i]);
}

console.log(source.slice(3, 1).length);
console.log(source.slice(10).length);
