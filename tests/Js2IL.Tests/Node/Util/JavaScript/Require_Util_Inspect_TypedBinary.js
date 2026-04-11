"use strict";

const util = require('util');

const ints = new Int32Array([10, 20, 30]);
const bytes = new Uint8Array([1, 2, 255]);
const arrayBuffer = new ArrayBuffer(6);
const dataView = new DataView(arrayBuffer, 1, 4);
const circular = { data: ints };
circular.self = circular;

console.log(util.inspect(ints));
console.log(util.inspect(bytes));
console.log(util.inspect(arrayBuffer));
console.log(util.inspect(dataView));
console.log(util.inspect(circular, { depth: 1 }));
