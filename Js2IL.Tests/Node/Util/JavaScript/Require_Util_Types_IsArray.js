"use strict";

const util = require('util');

// Test isArray
const arr = [1, 2, 3];
const notArr = { 0: 1, 1: 2, length: 2 };
const str = 'hello';

console.log(util.types.isArray(arr)); // true
console.log(util.types.isArray(notArr)); // false
console.log(util.types.isArray(str)); // false
