"use strict";

const buffer = new ArrayBuffer(10);
const view = new DataView(buffer, 2, 4);
const root = new DataView(buffer);

view.setUint8(0, 10);
view.setUint8(3, 20);

console.log(view.byteOffset);
console.log(view.byteLength);
console.log(view.buffer.byteLength);
console.log(root.getUint8(2));
console.log(root.getUint8(5));
