"use strict";

var AsyncFunction = async function foo() {}.constructor;

console.log(typeof AsyncFunction);
console.log(AsyncFunction === Function);
console.log(Object.getPrototypeOf(async function bar() {}) === AsyncFunction.prototype);
console.log(Object.getPrototypeOf(AsyncFunction.prototype) === Function.prototype);
console.log(AsyncFunction.prototype.constructor === AsyncFunction);
console.log(AsyncFunction.prototype[Symbol.toStringTag]);
