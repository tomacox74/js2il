"use strict";

const zlib = require("node:zlib");

const input = "Hello gzip baseline 🚀\nline two";
const compressed = zlib.gzipSync(input, { level: 9 });
const roundTrip = zlib.gunzipSync(compressed).toString("utf8");

console.log("compressed buffer:", Buffer.isBuffer(compressed));
console.log("compressed length > 0:", compressed.length > 0);
console.log("roundtrip:", roundTrip);
