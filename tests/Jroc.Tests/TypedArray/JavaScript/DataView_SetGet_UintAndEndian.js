"use strict";

const buffer = new ArrayBuffer(8);
const view = new DataView(buffer);

view.setUint8(0, 255);
view.setUint16(1, 0x1234);
view.setUint16(3, 0x1234, true);
view.setInt32(4, -1);

console.log(view.getUint8(0));
console.log(view.getUint16(1));
console.log(view.getUint16(3, true));
console.log(view.getUint16(3));
console.log(view.getInt32(4));
console.log(view.getUint32(4));
