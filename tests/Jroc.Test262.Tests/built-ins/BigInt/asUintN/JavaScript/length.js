// test262: test/built-ins/BigInt/asUintN/length.js
var descriptor = Object.getOwnPropertyDescriptor(BigInt.asUintN, "length");

console.log(descriptor.value === 2);
console.log(descriptor.enumerable === false);
console.log(descriptor.writable === false);
console.log(descriptor.configurable === true);
