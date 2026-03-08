"use strict";

var arrayLike = { 0: "a", 1: "b", length: 2 };
arrayLike[Symbol.isConcatSpreadable] = true;

var spreadableArray = [3, 4];
spreadableArray[Symbol.isConcatSpreadable] = false;

var notSpread = { 0: "x", length: 1 };
notSpread[Symbol.isConcatSpreadable] = false;

var nullSpreadable = [5, 6];
nullSpreadable[Symbol.isConcatSpreadable] = null;

var first = [1, 2].concat(arrayLike);
console.log(first.length);
console.log(first.join(","));

var second = [1, 2].concat(spreadableArray);
console.log(second.length);
console.log(Array.isArray(second[2]));
console.log(second[2].join(","));

var third = [1].concat(notSpread);
console.log(third.length);
console.log(third[1][0]);

var fourth = [1].concat(nullSpreadable);
console.log(fourth.length);
console.log(Array.isArray(fourth[1]));
console.log(fourth[1].join(","));
