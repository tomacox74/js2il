"use strict";

const array = new Int32Array(3);
const wordOffset = 64 >>> 5; // 64 / 32 = 2
array[wordOffset] = 42; // write using shift-derived index
console.log(array[wordOffset]); // expect 42
