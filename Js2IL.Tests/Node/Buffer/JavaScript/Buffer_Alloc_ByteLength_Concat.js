"use strict";

var zero = Buffer.alloc(4);
console.log(zero.length);
console.log(zero.toString("hex"));

var filled = Buffer.alloc(5, "ab");
console.log(filled.toString());

var numFill = Buffer.alloc(3, 65);
console.log(numFill.toString());

console.log(Buffer.byteLength("hello"));
console.log(Buffer.byteLength("6869", "hex"));
console.log(Buffer.byteLength("aGk=", "base64"));

var fromHex = Buffer.from("6869", "hex");
var fromBase64 = Buffer.from("aGk=", "base64");
console.log(fromHex.toString());
console.log(fromBase64.toString());

var joined = Buffer.concat([fromHex, Buffer.from("!"), fromBase64]);
console.log(joined.toString());

var truncated = Buffer.concat([Buffer.from("hello"), Buffer.from("world")], 7);
console.log(truncated.toString());
