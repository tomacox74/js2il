"use strict";

const util = require('util');

const arrayBuffer = new ArrayBuffer(8);
const dataView = new DataView(arrayBuffer, 2, 4);

console.log(util.types.isArrayBuffer(arrayBuffer));
console.log(util.types.isArrayBuffer(new Uint8Array(4)));
console.log(util.types.isAnyArrayBuffer(arrayBuffer));
console.log(util.types.isDataView(dataView));
console.log(util.types.isDataView(arrayBuffer));
console.log(util.types.isInt32Array(new Int32Array([1, 2, 3])));
console.log(util.types.isUint8Array(new Uint8Array([1, 2, 3])));
console.log(util.types.isUint8Array(Buffer.from([1, 2, 3])));
console.log(util.types.isFloat64Array(new Float64Array([1.5, 2.5])));
console.log(util.types.isUint16Array(new Uint8Array([1, 2, 3])));
console.log(util.types.isSharedArrayBuffer(arrayBuffer));
