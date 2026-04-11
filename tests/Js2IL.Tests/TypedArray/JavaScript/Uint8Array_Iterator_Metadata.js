"use strict";

const view = new Uint8Array([4, 5]);
const keys = view.keys();

console.log(keys.next().value);
console.log(keys.next().value);
console.log(keys.next().done);

const entry = view.entries().next().value;
console.log(entry[0]);
console.log(entry[1]);

console.log(view[Symbol.iterator]().next().value);
console.log(Object.prototype.toString.call(view));
console.log(view.BYTES_PER_ELEMENT);
