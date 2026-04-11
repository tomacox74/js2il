"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const transform = new stream.Transform();
const writable = new stream.Writable();

transform._transform = function (chunk) {
  this.push(chunk.toUpperCase());
};

let written = [];
writable._write = function (chunk) {
  written.push(chunk);
};

writable.on("finish", function () {
  console.log("finish");
  console.log("written:" + written.join(","));
});

readable.pipe(transform).pipe(writable);

readable.push("a");
readable.push("b");
readable.push(null);
