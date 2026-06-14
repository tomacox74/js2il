"use strict";

const pairs = [
  ["", ""],
  ["Zg==", "102"],
  ["Zm8=", "102|111"],
  ["Zm9v", "102|111|111"],
];

for (const [input, expected] of pairs) {
  const arr = Uint8Array.fromBase64(input);
  console.log(Object.getPrototypeOf(arr) === Uint8Array.prototype);
  console.log(arr.length);
  console.log(arr.buffer.byteLength);
  console.log(arr.join("|") === expected);
}
