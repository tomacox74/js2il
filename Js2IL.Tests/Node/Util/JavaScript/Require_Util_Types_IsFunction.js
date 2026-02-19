"use strict";

const util = require('util');

// Test isFunction
function regularFn() {}
const arrowFn = () => {};
const notFn = 42;
const obj = {};

console.log(util.types.isFunction(regularFn)); // true
console.log(util.types.isFunction(arrowFn)); // true
console.log(util.types.isFunction(notFn)); // false
console.log(util.types.isFunction(obj)); // false
