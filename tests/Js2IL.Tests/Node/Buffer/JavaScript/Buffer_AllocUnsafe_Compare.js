"use strict";

// Test allocUnsafe
var buf1 = Buffer.allocUnsafe(5);
console.log(buf1.length);
console.log(Buffer.isBuffer(buf1));

// Test compare
var buf2 = Buffer.from([1, 2, 3]);
var buf3 = Buffer.from([1, 2, 3]);
var buf4 = Buffer.from([1, 2, 4]);
var buf5 = Buffer.from([1, 2]);

console.log(Buffer.compare(buf2, buf3));
console.log(Buffer.compare(buf2, buf4));
console.log(Buffer.compare(buf4, buf2));
console.log(Buffer.compare(buf2, buf5));
console.log(Buffer.compare(buf5, buf2));
