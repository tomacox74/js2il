"use strict";

const view = new Uint8Array([1, 2, 3]);
const iter = view.values();

console.log(iter.next().value);
console.log(iter.next().value);
console.log(iter.next().value);
console.log(iter.next().done);

for (const value of view) {
  console.log(value);
}
