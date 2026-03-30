"use strict";

const stream = require("node:stream");

const writable = new stream.Writable();
writable._write = function () {};

stream.finished(writable, function (err) {
  console.log("callback:" + (err ? err.message : "null"));
  console.log("destroyed:" + writable.destroyed);
});

writable.end("done");
console.log("after-end:" + writable.writable);
