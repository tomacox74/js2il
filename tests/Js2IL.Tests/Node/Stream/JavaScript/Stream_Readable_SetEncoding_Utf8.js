"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();

readable.push(Buffer.from([0xE2, 0x82]));
readable.push(Buffer.from([0xAC]));
readable.setEncoding("utf8");

const buffered = readable.read();
console.log("buffered:" + buffered);
console.log("buffered-type:" + typeof buffered);

readable.on("data", function (chunk) {
  console.log("data:" + chunk + ":" + typeof chunk);
});

readable.on("end", function () {
  console.log("encoding:" + readable.readableEncoding);
});

readable.push(Buffer.from(" ok"));
readable.push(null);
