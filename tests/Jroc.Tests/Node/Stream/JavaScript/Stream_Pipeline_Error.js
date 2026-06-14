"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const transform = new stream.Transform();
const writable = new stream.Writable();

transform._transform = function () {
  throw new Error("transform boom");
};

writable._write = function () {};

stream.pipeline(readable, transform, writable, function (err) {
  console.log("callback:" + (err ? err.message : "null"));
  console.log("source-destroyed:" + readable.destroyed);
  console.log("transform-destroyed:" + transform.destroyed);
  console.log("writable-destroyed:" + writable.destroyed);
});

readable.push("a");
