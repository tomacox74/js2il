"use strict";

console.log(globalThis.WeakMap === WeakMap);
console.log(WeakMap.prototype.constructor === WeakMap);

var weakMap = new WeakMap();
console.log(Object.getPrototypeOf(weakMap) === WeakMap.prototype);
console.log(weakMap instanceof WeakMap);
console.log(weakMap.constructor === WeakMap);

var key = {};
WeakMap.prototype.set.call(weakMap, key, 7);
console.log(WeakMap.prototype.get.call(weakMap, key));
