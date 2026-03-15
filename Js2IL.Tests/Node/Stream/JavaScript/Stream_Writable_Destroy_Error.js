"use strict";

const stream = require("node:stream");

const writable = new stream.Writable();
const sequence = [];

writable._write = function (chunk) {
  throw new Error("boom:" + chunk);
};

writable.on("error", function (err) {
  sequence.push("error:" + err.message);
});

writable.on("finish", function () {
  sequence.push("finish");
});

writable.on("close", function () {
  sequence.push("close");
  console.log(sequence.join(","));
  console.log("destroyed:" + writable.destroyed);
});

console.log("write-return:" + writable.write("x"));
