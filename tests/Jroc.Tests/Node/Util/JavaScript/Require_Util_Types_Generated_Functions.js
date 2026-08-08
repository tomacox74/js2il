"use strict";

const util = require("node:util");

function ordinary() {}
async function asyncFunction() {}
function* generatorFunction() {}

console.log("ordinary:" + util.types.isFunction(ordinary));
console.log("async-function:" + util.types.isFunction(asyncFunction));
console.log("generator-function:" + util.types.isFunction(generatorFunction));
console.log("is-async:" + util.types.isAsyncFunction(asyncFunction));
console.log("ordinary-is-async:" + util.types.isAsyncFunction(ordinary));
