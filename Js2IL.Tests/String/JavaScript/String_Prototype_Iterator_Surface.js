"use strict";

console.log(typeof String.prototype);
console.log(String.prototype.constructor === String);
console.log(String.prototype.toString.call("abc"));
console.log(String.prototype.valueOf.call("abc"));

const borrowedAt = "abc".at;
console.log(typeof borrowedAt);
console.log(borrowedAt.call("xyz", 1));
console.log(typeof "abc"[Symbol.iterator]);

const iterator = String.prototype[Symbol.iterator].call("A😀B");
console.log(iterator.next().value);
console.log(iterator.next().value);
console.log(iterator.next().value);
console.log(iterator.next().done);

const iteratorPrototype = Object.getPrototypeOf(iterator);
console.log(typeof iteratorPrototype.next);
console.log(iteratorPrototype[Symbol.toStringTag]);
console.log(iterator[Symbol.iterator]() === iterator);
console.log(Array.from("A😀B").join("|"));
