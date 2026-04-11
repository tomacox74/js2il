"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const pass = new stream.PassThrough();
const writable = new stream.Writable();
const written = [];

writable._write = function (chunk) {
  written.push(chunk);
};

stream.pipeline(readable, pass, writable, function (err) {
  console.log("callback:" + (err ? err.message : "null"));
  console.log("written:" + written.join(","));
  console.log("pass-destroyed:" + pass.destroyed);
});

readable.push("a");
readable.push("b");
readable.push(null);
