"use strict";

const buffer = new ArrayBuffer(16);
const view = new DataView(buffer);

view.setFloat32(0, 1.25);
view.setFloat64(8, 3.141592653589793, true);

console.log(view.getFloat32(0));
console.log(view.getFloat64(8, true));
