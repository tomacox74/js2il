"use strict";

const util = require('util');

// Test isPromise
const p1 = new Promise((resolve) => resolve(42));
const p2 = { then: () => {} }; // Promise-like but not a real Promise
const notPromise = 42;

console.log(util.types.isPromise(p1)); // true
console.log(util.types.isPromise(p2)); // false
console.log(util.types.isPromise(notPromise)); // false
