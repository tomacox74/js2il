"use strict";

const TypedArray = Object.getPrototypeOf(Int8Array);
const desc = Object.getOwnPropertyDescriptor(TypedArray, "prototype");

console.log(TypedArray === Object.getPrototypeOf(Float64Array));
console.log(TypedArray === Object.getPrototypeOf(Uint8Array));
console.log(desc.writable);
console.log(desc.enumerable);
console.log(desc.configurable);
