"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const pass = new stream.PassThrough();
const writable = new stream.Writable();

let written = [];
writable._write = function (chunk) {
  written.push(chunk);
};

writable.on("finish", function () {
  console.log("finish");
  console.log(written.length);
  console.log(written[0]);
  console.log(written[1]);
});

readable.pipe(pass).pipe(writable);

readable.push("a");
readable.push("b");
readable.push(null);
