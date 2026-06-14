"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const pass = new stream.PassThrough();
const writable = new stream.Writable();

writable._write = function () {
  throw new Error("write boom");
};

readable.on("error", function (err) {
  console.log("source-error:" + err.message);
});

pass.on("error", function (err) {
  console.log("pass-error:" + err.message);
});

writable.on("error", function (err) {
  console.log("writable-error:" + err.message);
});

stream.pipeline(readable, pass, writable, function (err) {
  console.log("callback:" + (err ? err.message : "null"));
  console.log("source-destroyed:" + readable.destroyed);
  console.log("pass-destroyed:" + pass.destroyed);
  console.log("writable-destroyed:" + writable.destroyed);
});

readable.push("a");
