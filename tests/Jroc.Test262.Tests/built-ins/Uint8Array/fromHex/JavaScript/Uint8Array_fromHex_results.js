"use strict";

[
  "",
  "66",
  "666f",
  "666F",
  "666f6f",
  "666F6f",
  "666f6f62",
  "666f6f6261",
  "666f6f626172"
].forEach(function (hex) {
  var arr = Uint8Array.fromHex(hex);
  console.log((Object.getPrototypeOf(arr) === Uint8Array.prototype) + "|" + arr.length + "|" + arr.buffer.byteLength + "|" + arr.join(","));
});
