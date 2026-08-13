"use strict";

const buffer = new ArrayBuffer(4, { maxByteLength: 8 });
const view = new Uint8Array(buffer);
const dataView = new DataView(buffer);
view[0] = 42;

console.log(buffer.resizable);
console.log(buffer.maxByteLength);

buffer.resize(8);
console.log(view.length);
console.log(dataView.byteLength);
console.log(view[0]);
console.log(view[4]);

let resizeRejected = false;
try {
  buffer.resize(9);
} catch (error) {
  resizeRejected = true;
}
console.log(resizeRejected);

buffer.resize(0);
console.log(view.length);
console.log(dataView.byteLength);
