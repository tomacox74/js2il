"use strict";

var map = new Map([
    ["x", 10],
    ["y", 20]
]);

var iterator = map[Symbol.iterator]();
var first = iterator.next();
var second = iterator.next();
var third = iterator.next();

console.log(first.done);
console.log(first.value[0]);
console.log(first.value[1]);
console.log(second.done);
console.log(second.value[0]);
console.log(second.value[1]);
console.log(third.done);

