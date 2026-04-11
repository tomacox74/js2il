"use strict";

var set = new Set([4, 5]);
var iterator = set[Symbol.iterator]();
var first = iterator.next();
var second = iterator.next();
var third = iterator.next();

console.log(first.done);
console.log(first.value);
console.log(second.done);
console.log(second.value);
console.log(third.done);

