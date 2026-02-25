"use strict";

function namedFn(a, b) {
    return a + b;
}

const source = Function.prototype.toString.call(namedFn);
console.log(typeof source);
console.log(source.indexOf("function") === 0);
console.log(source.indexOf("[native code]") >= 0);
