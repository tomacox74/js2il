// test262: test/built-ins/BigInt/asUintN/asUintN.js
var descriptor = Object.getOwnPropertyDescriptor(BigInt, "asUintN");

console.log(typeof BigInt.asUintN === "function");
console.log(typeof descriptor === "object");
console.log(descriptor.enumerable === false);
console.log(descriptor.writable === true);
console.log(descriptor.configurable === true);
