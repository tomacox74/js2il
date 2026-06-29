"use strict";

const stringDecoder = require("node:string_decoder");
const decoder = new stringDecoder.StringDecoder("utf-8");

console.log(decoder.write(Buffer.from("ok")));
console.log(decoder.end(Buffer.from("!")));
