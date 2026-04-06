"use strict";

console.log(globalThis.WeakSet === WeakSet);
console.log(WeakSet.prototype.constructor === WeakSet);
console.log(WeakSet instanceof Function);
console.log(Object.getPrototypeOf(WeakSet) === Function.prototype);
console.log(Object.getPrototypeOf(WeakSet.prototype) === Object.prototype);

var descriptor = Object.getOwnPropertyDescriptor(WeakSet, "prototype");
console.log(descriptor.writable);
console.log(descriptor.enumerable);
console.log(descriptor.configurable);

var weakSet = new WeakSet();
console.log(Object.getPrototypeOf(weakSet) === WeakSet.prototype);
console.log(weakSet instanceof WeakSet);
console.log(weakSet.constructor === WeakSet);

var value = {};
WeakSet.prototype.add.call(weakSet, value);
console.log(WeakSet.prototype.has.call(weakSet, value));

try {
  var weakSetCtor = WeakSet;
  weakSetCtor();
  console.log("no-throw");
} catch (e) {
  console.log(e.name + ": " + e.message);
}
