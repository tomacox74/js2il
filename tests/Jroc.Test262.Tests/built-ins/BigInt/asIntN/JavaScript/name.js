// test262: test/built-ins/BigInt/asIntN/name.js
var descriptor = Object.getOwnPropertyDescriptor(BigInt.asIntN, "name");

console.log(descriptor.value === "asIntN");
console.log(descriptor.enumerable === false);
console.log(descriptor.writable === false);
console.log(descriptor.configurable === true);
