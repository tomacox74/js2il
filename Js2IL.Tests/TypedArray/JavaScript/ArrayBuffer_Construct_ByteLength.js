"use strict";

const buffer = new ArrayBuffer(8);
const sliced = buffer.slice(2, 6);

console.log(buffer.byteLength);
console.log(sliced.byteLength);
