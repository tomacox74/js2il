"use strict";

const buffer = new ArrayBuffer(4);
const view = new DataView(buffer);
const typed = new Int32Array(2);

console.log(ArrayBuffer.isView(buffer));
console.log(ArrayBuffer.isView(view));
console.log(ArrayBuffer.isView(typed));
