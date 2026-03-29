"use strict";

console.log(globalThis.Set === Set);
console.log(Set.prototype.constructor === Set);

var set = new Set();
console.log(Object.getPrototypeOf(set) === Set.prototype);
console.log(set instanceof Set);
console.log(set.constructor === Set);

Set.prototype.add.call(set, "value");
console.log(Set.prototype.has.call(set, "value"));
