"use strict";

console.log(globalThis.WeakMap === WeakMap);
console.log(WeakMap.prototype.constructor === WeakMap);
console.log(WeakMap instanceof Function);
console.log(Object.getPrototypeOf(WeakMap) === Function.prototype);
console.log(Object.getPrototypeOf(WeakMap.prototype) === Object.prototype);

var descriptor = Object.getOwnPropertyDescriptor(WeakMap, "prototype");
console.log(descriptor.writable);
console.log(descriptor.enumerable);
console.log(descriptor.configurable);

var weakMap = new WeakMap();
console.log(Object.getPrototypeOf(weakMap) === WeakMap.prototype);
console.log(weakMap instanceof WeakMap);
console.log(weakMap.constructor === WeakMap);

var key = {};
WeakMap.prototype.set.call(weakMap, key, 7);
console.log(WeakMap.prototype.get.call(weakMap, key));

try {
  var weakMapCtor = WeakMap;
  weakMapCtor();
  console.log("no-throw");
} catch (e) {
  console.log(e.name + ": " + e.message);
}
