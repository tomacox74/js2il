"use strict";

const StringDecoder = require("string_decoder").StringDecoder;
const decoder = new StringDecoder("utf8");

console.log("encoding:" + decoder.encoding);
console.log("first:" + decoder.write(Buffer.from([0xE2, 0x82])));
console.log("second:" + decoder.write(Buffer.from([0xAC, 0x20, 0x41])));
console.log("end:" + decoder.end());

const dangling = new StringDecoder();
console.log("dangling-write:" + dangling.write(Buffer.from([0xE2])));
console.log("dangling-end:" + dangling.end());
