"use strict";

const stream = require("node:stream");
const streamPromises = require("node:stream/promises");

async function main() {
  const readable = new stream.Readable({ objectMode: true });
  const transform = new stream.Transform({ objectMode: true });
  const writable = new stream.Writable({ objectMode: true });
  const written = [];

  transform._transform = function (chunk) {
    this.push({ value: chunk.value + 1 });
  };

  writable._write = function (chunk) {
    written.push(chunk.value);
  };

  const completion = streamPromises.pipeline(readable, transform, writable);

  readable.push({ value: 1 });
  readable.push({ value: 2 });
  readable.push(null);

  await completion;

  console.log("readableObjectMode:" + readable.readableObjectMode);
  console.log("transformReadableObjectMode:" + transform.readableObjectMode);
  console.log("transformWritableObjectMode:" + transform.writableObjectMode);
  console.log("writableObjectMode:" + writable.writableObjectMode);
  console.log("written:" + written.join(","));
}

main();
