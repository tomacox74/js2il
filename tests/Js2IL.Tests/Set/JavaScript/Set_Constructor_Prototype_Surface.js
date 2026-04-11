"use strict";

console.log(globalThis.Set === Set);
console.log(Set.prototype.constructor === Set);
console.log(Set instanceof Function);
console.log(Object.getPrototypeOf(Set) === Function.prototype);
console.log(Object.getPrototypeOf(Set.prototype) === Object.prototype);

var descriptor = Object.getOwnPropertyDescriptor(Set, "prototype");
console.log(descriptor.writable);
console.log(descriptor.enumerable);
console.log(descriptor.configurable);

var set = new Set();
console.log(Object.getPrototypeOf(set) === Set.prototype);
console.log(set instanceof Set);
console.log(set.constructor === Set);

Set.prototype.add.call(set, "value");
console.log(Set.prototype.has.call(set, "value"));

try {
  var setCtor = Set;
  setCtor();
  console.log("no-throw");
} catch (e) {
  console.log(e.name + ": " + e.message);
}
