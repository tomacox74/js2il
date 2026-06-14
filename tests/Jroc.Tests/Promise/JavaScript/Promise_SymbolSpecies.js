"use strict";

console.log(Promise[Symbol.species] === Promise);

var descriptor = Object.getOwnPropertyDescriptor(Promise, Symbol.species);
console.log(typeof descriptor.get === "function");
console.log(descriptor.set === undefined);
console.log(descriptor.enumerable);
console.log(descriptor.configurable);
