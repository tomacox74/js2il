"use strict";

const stream = require("node:stream");
const Readable = stream.Readable;
const Writable = stream.Writable;

const readable = new Readable();
const writable = new Writable();

let writtenData = [];

// Override _write to capture data
writable._write = function(chunk) {
  writtenData.push(chunk);
};

// Test pipe
readable.pipe(writable);

// Push data through readable
readable.push("data1");
readable.push("data2");
readable.push("data3");
readable.push(null); // End

// Output results
console.log("Written data count:", writtenData.length);
for (let i = 0; i < writtenData.length; i++) {
  console.log("Chunk " + i + ":", writtenData[i]);
}
