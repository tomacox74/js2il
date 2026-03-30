"use strict";

// Search/position Array operations

var arr = [1, 2, 3, 2];
console.log(arr.indexOf(2));
console.log(arr.lastIndexOf(2));
console.log(arr.findIndex(function (x) { return x === 2; }));
console.log(arr.findLast(function (x) { return x === 2; }));
console.log(arr.findLastIndex(function (x) { return x === 2; }));

var i = [1, 2, 3];
console.log(i.at(-1));
console.log(i.at(0));

console.log(arr.includes(3));
console.log(arr.includes(9));
