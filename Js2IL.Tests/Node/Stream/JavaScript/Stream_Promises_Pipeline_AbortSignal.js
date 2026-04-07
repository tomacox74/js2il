"use strict";

const stream = require("node:stream");
const streamPromises = require("node:stream/promises");

function createController() {
  const signal = {
    aborted: false,
    reason: null,
    addEventListener: function (type, listener) {
      if (type === "abort") {
        this.listener = listener;
      }
    },
    removeEventListener: function (type, listener) {
      if (type === "abort" && this.listener === listener) {
        this.listener = null;
      }
    }
  };

  return {
    signal: signal,
    abort: function (reason) {
      signal.aborted = true;
      signal.reason = reason;
      if (signal.listener) {
        signal.listener();
      }
    }
  };
}

const controller = createController();
const readable = new stream.Readable({ objectMode: true });
const pass = new stream.PassThrough({ objectMode: true });
const writable = new stream.Writable({ objectMode: true });
const written = [];

writable._write = function (chunk) {
  written.push(chunk.value);
};

const completion = streamPromises.pipeline(readable, pass, writable, { signal: controller.signal });

readable.push({ value: 1 });
controller.abort("stop");

completion
  .then(function () {
    console.log("no-abort");
  })
  .catch(function (error) {
    console.log(error.name);
    console.log(error.code);
    console.log(error.message);
  })
  .then(function () {
  console.log("written:" + written.join(","));
  console.log("readable-destroyed:" + readable.destroyed);
  console.log("pass-destroyed:" + pass.destroyed);
  console.log("writable-destroyed:" + writable.destroyed);
  });
