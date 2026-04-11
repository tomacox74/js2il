"use strict";

const stream = require("node:stream");
const Readable = stream.Readable;

const readable = new Readable();

// Test readable property
console.log(readable.readable);

// Test push and data event
let dataReceived = 0;
readable.on("data", function(chunk) {
  console.log("Data received:", chunk);
  dataReceived++;
});

readable.on("end", function() {
  console.log("Stream ended");
});

// Push some data
readable.push("chunk1");
readable.push("chunk2");
readable.push(null); // Signal end

console.log("Data events:", dataReceived);
console.log("Readable after end:", readable.readable);
