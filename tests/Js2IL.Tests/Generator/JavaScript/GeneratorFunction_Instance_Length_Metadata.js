"use strict";

const GeneratorFunction = Object.getPrototypeOf(function* sample(a, b) {}).constructor;
const lengthDesc = Object.getOwnPropertyDescriptor(GeneratorFunction, "length");

console.log(typeof GeneratorFunction);
console.log(GeneratorFunction.length);
console.log(lengthDesc.value);
console.log(lengthDesc.writable);
console.log(lengthDesc.enumerable);
console.log(lengthDesc.configurable);
