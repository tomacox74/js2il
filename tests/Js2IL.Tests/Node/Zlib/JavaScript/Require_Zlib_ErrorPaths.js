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

console.log(
  "gzip level -1 roundtrip:",
  zlib.gunzipSync(zlib.gzipSync("abc", { level: -1 })).toString("utf8")
);
console.log(
  "gzip level 1.5 roundtrip:",
  zlib.gunzipSync(zlib.gzipSync("abc", { level: 1.5 })).toString("utf8")
);

logError("gzip unsupported option", () => zlib.gzipSync("abc", { chunkSize: 1024 }));
logError("gunzip unsupported option", () => zlib.gunzipSync(zlib.gzipSync("abc"), { flush: 2 }));
logError("gzip bad level", () => zlib.gzipSync("abc", { level: 10 }));
logError("gzip NaN level", () => zlib.gzipSync("abc", { level: NaN }));
logError("gzip Infinity level", () => zlib.gzipSync("abc", { level: Infinity }));

try {
  zlib.gunzipSync(Buffer.from("not gzip"));
  console.log("gunzip invalid data threw:", false);
} catch (error) {
  console.log("gunzip invalid data threw:", true);
  console.log("gunzip invalid data name:", error.name);
  console.log(
    "gunzip invalid data message length > 0:",
    typeof error.message === "string" && error.message.length > 0
  );
}
