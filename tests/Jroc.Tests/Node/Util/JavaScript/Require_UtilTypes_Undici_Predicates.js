"use strict";

const util = require("util");
const types = require("util/types");
const nodeTypes = require("node:util/types");
const { isArrayBuffer, isUint8Array } = nodeTypes;

const arrayBuffer = new ArrayBuffer(4);
const dataView = new DataView(arrayBuffer);

console.log(types === util.types);
console.log(nodeTypes === types);
console.log(isUint8Array(new Uint8Array(4)));
console.log(isUint8Array(Buffer.from([1, 2, 3])));
console.log(isUint8Array(new Int32Array(2)));
console.log(isUint8Array(dataView));
console.log(isUint8Array(arrayBuffer));
console.log(isArrayBuffer(arrayBuffer));
console.log(isArrayBuffer(new Uint8Array(4)));
console.log(isArrayBuffer(dataView));
