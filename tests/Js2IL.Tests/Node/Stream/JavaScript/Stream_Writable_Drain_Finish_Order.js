"use strict";

const stream = require("node:stream");

const writable = new stream.Writable();
const written = [];
const sequence = [];

writable.highWaterMark = 1;
writable._write = function (chunk) {
  written.push(chunk);
};

writable.on("drain", function () {
  sequence.push("drain");
});

writable.on("finish", function () {
  sequence.push("finish");
  console.log("sequence:" + sequence.join(","));
  console.log("written:" + written.join(","));
});

console.log("write:" + writable.write("a"));
writable.end("b");
console.log("after-end:" + writable.writable);
