// test262: test/built-ins/BigInt/asUintN/name.js
var descriptor = Object.getOwnPropertyDescriptor(BigInt.asUintN, "name");

console.log(descriptor.value === "asUintN");
console.log(descriptor.enumerable === false);
console.log(descriptor.writable === false);
console.log(descriptor.configurable === true);
