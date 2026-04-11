"use strict";

function target(a, b, c) {
    return a + b + c;
}

const boundOnce = target.bind({ ignored: true }, 1);
const boundTwice = boundOnce.bind({ ignoredAgain: true }, 2, 3);

const lengthDesc = Object.getOwnPropertyDescriptor(boundOnce, "length");
const nameDesc = Object.getOwnPropertyDescriptor(boundOnce, "name");

console.log(boundOnce.length);
console.log(boundOnce.name);
console.log(lengthDesc.value);
console.log(lengthDesc.writable);
console.log(lengthDesc.enumerable);
console.log(lengthDesc.configurable);
console.log(nameDesc.value);
console.log(nameDesc.writable);
console.log(nameDesc.enumerable);
console.log(nameDesc.configurable);
console.log(Object.hasOwn(boundOnce, "length"));
console.log(Object.hasOwn(boundOnce, "name"));
console.log(Object.hasOwn(boundOnce, "prototype"));
console.log(boundOnce.prototype === undefined);
console.log(boundTwice.length);
console.log(boundTwice.name);
