"use strict";

console.log(typeof Function.prototype.bind);

function add(a, b) {
    return a + b;
}

const add1 = Function.prototype.bind.call(add, null, 1);
console.log(add1(2));
