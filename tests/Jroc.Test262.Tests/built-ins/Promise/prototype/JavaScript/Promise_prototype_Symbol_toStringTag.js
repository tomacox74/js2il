"use strict";

const promise = Promise.resolve(1);

console.log(typeof Promise);
console.log(Promise.prototype[Symbol.toStringTag]);
console.log(Object.prototype.toString.call(Promise.prototype));
console.log(Object.prototype.toString.call(promise));
console.log(promise[Symbol.toStringTag]);
