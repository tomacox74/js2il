"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();

stream.finished(readable, function (err) {
  console.log("callback:" + (err ? err.message : "null"));
  console.log("destroyed:" + readable.destroyed);
});

readable.destroy(new Error("read boom"));
