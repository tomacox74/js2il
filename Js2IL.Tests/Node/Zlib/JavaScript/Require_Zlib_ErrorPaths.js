"use strict";

const zlib = require("node:zlib");

function logError(label, action) {
  try {
    action();
    console.log(label + " threw:", false);
  } catch (error) {
    console.log(label + " threw:", true);
    console.log(label + " name:", error.name);
    console.log(label + " message:", error.message);
  }
}

logError("gzip unsupported option", () => zlib.gzipSync("abc", { chunkSize: 1024 }));
logError("gunzip unsupported option", () => zlib.gunzipSync(zlib.gzipSync("abc"), { flush: 2 }));
logError("gzip bad level", () => zlib.gzipSync("abc", { level: 12 }));
