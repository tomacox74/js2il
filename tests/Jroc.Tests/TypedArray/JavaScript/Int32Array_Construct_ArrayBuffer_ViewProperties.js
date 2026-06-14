"use strict";

const buffer = new ArrayBuffer(12);
const view = new DataView(buffer);
view.setInt32(0, 11, true);
view.setInt32(4, -22, true);
view.setInt32(8, 33, true);

const typed = new Int32Array(buffer, 4);

console.log(typed.length);
console.log(typed.byteOffset);
console.log(typed.byteLength);
console.log(typed.buffer === buffer);
console.log(typed[0]);
typed[1] = 44;
console.log(view.getInt32(8, true));
