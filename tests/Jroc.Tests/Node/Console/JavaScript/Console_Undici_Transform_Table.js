"use strict";

const { Transform } = require("node:stream");
const { Console } = require("node:console");

const output = new Transform({
  transform(chunk, _encoding, callback) {
    callback(null, chunk);
  }
});

const logger = new Console({
  stdout: output,
  inspectOptions: { colors: false }
});

logger.table([
  { name: "first", value: 1 },
  { name: "second", value: 2 }
]);

console.log(output.read().toString());
