"use strict";

// Real-world libraries often do: Array.prototype.reduce.call(arrayLike, ...)
// (e.g., DOM NodeList, HTMLCollection, arguments, etc.)

var arrayLike = { 0: "a", 1: "b", 2: "c", length: 3 };

var reduced = Array.prototype.reduce.call(arrayLike, function (acc, x) { return acc + x; }, "");
console.log(reduced);

var reducedRight = Array.prototype.reduceRight.call(arrayLike, function (acc, x) { return acc + x; }, "");
console.log(reducedRight);

console.log(Array.prototype.indexOf.call(arrayLike, "b"));
console.log(Array.prototype.indexOf.call(arrayLike, "z"));

console.log(Array.prototype.indexOf.call(arrayLike, "a", 1));
console.log(Array.prototype.indexOf.call(arrayLike, "b", -2));
