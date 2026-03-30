"use strict";

console.log(Object.is(NaN, NaN));
console.log(Object.is(0, -0));
console.log(Object.is(-0, -0));
console.log(Object.is(1, 1));
console.log(Object.is(BigInt(10), BigInt(10)));
console.log(Object.is(BigInt(10), BigInt(11)));
