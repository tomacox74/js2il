"use strict";

const { Buffer, isUtf8, resolveObjectURL } = require("node:buffer");
const legacyBuffer = require("buffer");

const wasmBytes = Buffer.from("AGFzbQ==", "base64");
const packet = Buffer.concat([Buffer.from([1, 2]), Buffer.alloc(2)]);
packet.writeUInt16BE(8080, 2);

console.log(legacyBuffer.Buffer === Buffer);
console.log(wasmBytes.toString("hex"));
console.log(packet.readUInt16BE(2));
console.log(packet.subarray(0, 2).toString("hex"));
console.log(isUtf8(Buffer.from("valid")));
console.log(isUtf8(Buffer.from([0xc3, 0x28])));
console.log(packet.byteLength);
console.log(packet.buffer.byteLength);
console.log(Array.from(packet).join(","));
console.log(new Uint8Array(packet).length);
console.log(typeof resolveObjectURL);
