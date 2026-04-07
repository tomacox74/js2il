"use strict";

const stream = require("node:stream");
const streamPromises = require("node:stream/promises");

function createWritable() {
  const writable = new stream.Writable({ objectMode: true });
  writable._write = function (_chunk) {};
  return writable;
}

function createReadable() {
  return new stream.Readable({ objectMode: true });
}

async function main() {
  try {
    stream.finished(createWritable(), { signal: null }, function () {});
    console.log("callback-finished:ok");
  } catch (error) {
    console.log("callback-finished:" + error.name + ":" + error.code);
    console.log(error.message);
  }

  try {
    stream.pipeline(createReadable(), createWritable(), { signal: {} }, function () {});
    console.log("callback-pipeline:ok");
  } catch (error) {
    console.log("callback-pipeline:" + error.name + ":" + error.code);
    console.log(error.message);
  }

  try {
    await streamPromises.finished(createWritable(), { signal: null });
    console.log("promise-finished:ok");
  } catch (error) {
    console.log("promise-finished:" + error.name + ":" + error.code);
    console.log(error.message);
  }

  try {
    const readable = createReadable();
    const completion = streamPromises.pipeline(readable, createWritable(), { signal: {} });
    readable.push(null);
    await completion;
    console.log("promise-pipeline:ok");
  } catch (error) {
    console.log("promise-pipeline:" + error.name + ":" + error.code);
    console.log(error.message);
  }
}

main();
