"use strict";

const stream = require("node:stream");
const streamPromises = require("node:stream/promises");

async function main() {
  const writable = new stream.Writable();
  writable._write = function () {};

  const completion = streamPromises.finished(writable);
  writable.end("done");
  await completion;

  console.log("destroyed:" + writable.destroyed);
  console.log("writable:" + writable.writable);
}

main();
