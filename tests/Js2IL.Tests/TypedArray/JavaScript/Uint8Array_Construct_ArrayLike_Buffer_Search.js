"use strict";

const fromArray = new Uint8Array([255, 256, -1, 1.9]);
console.log(fromArray.length);
for (let i = 0; i < fromArray.length; i++) {
  console.log(fromArray[i]);
}

const buffer = new ArrayBuffer(4);
const view = new Uint8Array(buffer, 1, 2);
view[0] = 42;
view[1] = 43;

console.log(view.byteOffset);
console.log(view.byteLength);
console.log(view.buffer === buffer);

const full = new Uint8Array(buffer);
console.log(full[1]);
console.log(full[2]);
console.log(view.includes(43));
console.log(view.indexOf(42));
console.log(view.at(-1));
