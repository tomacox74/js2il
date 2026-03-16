"use strict";

const stream = require("node:stream");
const zlib = require("node:zlib");

const readable = new stream.Readable();
const gzip = zlib.createGzip({ level: -1 });
const gunzip = zlib.createGunzip();
const writable = new stream.Writable();

const chunks = [];
const events = [];

gzip.on("finish", () => events.push("gzip:finish"));
gzip.on("end", () => events.push("gzip:end"));
gunzip.on("finish", () => events.push("gunzip:finish"));
gunzip.on("end", () => events.push("gunzip:end"));

writable._write = function (chunk) {
  chunks.push(chunk);
};

writable.on("finish", function () {
  events.push("writable:finish");
  console.log("output:", Buffer.concat(chunks).toString("utf8"));
  console.log("events:", events.join(","));
});

readable.pipe(gzip).pipe(gunzip).pipe(writable);
readable.push(Buffer.from("stream "));
readable.push(Buffer.from("roundtrip"));
readable.push(null);
