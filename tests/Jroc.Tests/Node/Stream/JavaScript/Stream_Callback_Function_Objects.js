"use strict";

const stream = require("node:stream");

const asyncWritable = new stream.Writable();
asyncWritable._write = function () {};
stream.finished(asyncWritable, async function (error) {
  console.log("async:" + (error ? error.message : "null"));
});
asyncWritable.end("async");

const generatorWritable = new stream.Writable();
generatorWritable._write = function () {};
stream.finished(generatorWritable, function* (error) {
  yield error;
});
generatorWritable.end("generator");

setImmediate(function () {
  console.log("generator:accepted");
});
