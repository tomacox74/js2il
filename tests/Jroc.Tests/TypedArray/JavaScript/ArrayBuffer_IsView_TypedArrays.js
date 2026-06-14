"use strict";

console.log(ArrayBuffer.isView(new Int32Array(1)));
console.log(ArrayBuffer.isView(new Uint8Array(1)));
console.log(ArrayBuffer.isView(new Float64Array(1)));
console.log(ArrayBuffer.isView(new ArrayBuffer(8)));
