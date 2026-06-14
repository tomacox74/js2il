"use strict";

const buffer = new ArrayBuffer(24);
const view = new Float64Array(buffer, 8, 2);

console.log(view.length);
console.log(view.byteOffset);
console.log(view.byteLength);
console.log(view.buffer === buffer);

view[0] = 1.5;
view[1] = NaN;

const full = new Float64Array(buffer);
console.log(full[0]);
console.log(full[1]);
console.log(full[2]);
console.log(full.includes(NaN));
console.log(full.indexOf(NaN));
console.log(view.at(-1));

const copy = full.slice(1, 3);
console.log(copy.buffer === buffer);
console.log(copy[0]);
console.log(copy[1]);
