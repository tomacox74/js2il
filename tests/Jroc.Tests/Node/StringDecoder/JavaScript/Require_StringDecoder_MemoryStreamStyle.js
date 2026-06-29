"use strict";

const StringDecoder = require("string_decoder").StringDecoder;

function MemoryWritableStream() {
  this.decodeStrings = true;
}

MemoryWritableStream.prototype.decode = function (chunk, encoding) {
  const decoder = this.decodeStrings && encoding ? new StringDecoder(encoding) : null;
  return decoder ? decoder.write(chunk) + decoder.end() : chunk;
};

const stream = new MemoryWritableStream();
console.log(stream instanceof MemoryWritableStream);
console.log(stream.decode(Buffer.from([0xE2, 0x82, 0xAC]), "utf8"));
