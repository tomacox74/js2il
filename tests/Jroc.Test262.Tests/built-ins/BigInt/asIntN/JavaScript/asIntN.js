// test262: test/built-ins/BigInt/asIntN/asIntN.js
var descriptor = Object.getOwnPropertyDescriptor(BigInt, "asIntN");

console.log(typeof BigInt.asIntN === "function");
console.log(typeof descriptor === "object");
console.log(descriptor.enumerable === false);
console.log(descriptor.writable === true);
console.log(descriptor.configurable === true);
