"use strict";

const stream = require("node:stream");
const Writable = stream.Writable;

const writable = new Writable();

// Test writable property
console.log(writable.writable);

// Test drain event
writable.on("drain", function() {
  console.log("Drain event");
});

// Test finish event
writable.on("finish", function() {
  console.log("Finish event");
});

// Write some data
console.log(writable.write("chunk1"));
console.log(writable.write("chunk2"));

// End the stream
writable.end("final chunk");

console.log("Writable after end:", writable.writable);
