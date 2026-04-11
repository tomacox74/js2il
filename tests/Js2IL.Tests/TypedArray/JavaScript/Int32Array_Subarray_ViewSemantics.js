"use strict";

const source = new Int32Array([10, 20, 30, 40]);
const sub = source.subarray(1, 3);

console.log(sub.length);
console.log(sub.byteOffset);
console.log(sub.byteLength);
console.log(sub.buffer === source.buffer);

sub[0] = 99;
console.log(source[1]);

const copy = sub.slice();
console.log(copy.buffer === source.buffer);
copy[0] = -7;
console.log(sub[0]);
console.log(copy[0]);
