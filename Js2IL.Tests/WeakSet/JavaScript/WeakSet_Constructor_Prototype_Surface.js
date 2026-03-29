"use strict";

console.log(globalThis.WeakSet === WeakSet);
console.log(WeakSet.prototype.constructor === WeakSet);

var weakSet = new WeakSet();
console.log(Object.getPrototypeOf(weakSet) === WeakSet.prototype);
console.log(weakSet instanceof WeakSet);
console.log(weakSet.constructor === WeakSet);

var value = {};
WeakSet.prototype.add.call(weakSet, value);
console.log(WeakSet.prototype.has.call(weakSet, value));
