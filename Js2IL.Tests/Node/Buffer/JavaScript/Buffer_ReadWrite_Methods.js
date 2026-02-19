"use strict";

// Test read methods
var buf = Buffer.from([0x01, 0xFF, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC]);

console.log(buf.readInt8(0));
console.log(buf.readInt8(1));
console.log(buf.readUInt8(0));
console.log(buf.readUInt8(1));

console.log(buf.readInt16BE(0));
console.log(buf.readInt16LE(0));
console.log(buf.readUInt16BE(0));
console.log(buf.readUInt16LE(0));

console.log(buf.readInt32BE(2));
console.log(buf.readInt32LE(2));
console.log(buf.readUInt32BE(2));
console.log(buf.readUInt32LE(2));

// Test write methods
var buf2 = Buffer.alloc(8);
buf2.writeInt8(127, 0);
buf2.writeInt8(-128, 1);
console.log(buf2.readInt8(0));
console.log(buf2.readInt8(1));

buf2.writeUInt8(255, 2);
console.log(buf2.readUInt8(2));

buf2.writeInt16BE(0x1234, 3);
console.log(buf2.readInt16BE(3));

buf2.writeInt32LE(-1, 0);
console.log(buf2.readInt32LE(0));

// Test write string
var buf3 = Buffer.alloc(10);
var written = buf3.write("hello", 0, 5);
console.log(written);
console.log(buf3.toString("utf8", 0, 5));

var written2 = buf3.write("world", 5);
console.log(written2);
console.log(buf3.toString());
