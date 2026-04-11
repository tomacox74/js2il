"use strict";

const stream = require("node:stream");
const Writable = stream.Writable;

const writable = new Writable();

let writtenData = [];

// Set _write to capture data
writable._write = function(chunk) {
  writtenData.push(chunk);
  console.log("_write called with:", chunk);
};

// Write some data
writable.write("test1");
writable.write("test2");
writable.end();

console.log("Written data count:", writtenData.length);
for (let i = 0; i < writtenData.length; i++) {
  console.log("Chunk " + i + ":", writtenData[i]);
}
