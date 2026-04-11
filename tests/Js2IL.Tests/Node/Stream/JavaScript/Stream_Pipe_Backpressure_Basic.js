"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const writable = new stream.Writable();

writable.highWaterMark = 1;

let written = [];
writable._write = function (chunk) {
  written.push(chunk);
};

writable.on("drain", function () {
  console.log("drain");
});

writable.on("finish", function () {
  console.log("finish");
  console.log("written:" + written.join(","));
});

const chunks = ["a", "b", "c"];
let index = 0;

writable.on("drain", function () {
  index++;
  if (index < chunks.length) {
    readable.push(chunks[index]);
  } else {
    readable.push(null);
  }
});

readable.pipe(writable);
readable.push(chunks[0]);
