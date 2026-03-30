"use strict";

const crypto = require("crypto");

const bytes = new Uint8Array(4);
const returnedBytes = crypto.webcrypto.getRandomValues(bytes);
console.log("same uint8:", returnedBytes === bytes);
console.log("uint8 length:", bytes.length);

const ints = new Int32Array(2);
const returnedInts = crypto.getRandomValues(ints);
console.log("same int32:", returnedInts === ints);
console.log("int32 byteLength:", ints.byteLength);

const buffer = Buffer.alloc(6);
const returnedBuffer = crypto.webcrypto.getRandomValues(buffer);
console.log("same buffer:", returnedBuffer === buffer);
console.log("buffer length:", buffer.length);
console.log("buffer hex length:", buffer.toString("hex").length);
