"use strict";

const stream = require("node:stream");

const readable = new stream.Readable();
const events = [];

readable.on("pause", function () {
  events.push("pause");
});

readable.on("resume", function () {
  events.push("resume");
});

readable.on("data", function (chunk) {
  console.log("data:" + chunk);
});

readable.on("end", function () {
  console.log("end");
  console.log("events:" + events.join(","));
});

readable.pause();
readable.push("a");
readable.push("b");
console.log("paused");
readable.resume();
readable.push("c");
readable.push(null);
