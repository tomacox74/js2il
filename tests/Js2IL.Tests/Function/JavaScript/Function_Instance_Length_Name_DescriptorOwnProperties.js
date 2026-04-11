"use strict";

function namedAdd(a, b, c) {
    return a + b + c;
}

const lengthDesc = Object.getOwnPropertyDescriptor(namedAdd, "length");
const nameDesc = Object.getOwnPropertyDescriptor(namedAdd, "name");

console.log("named length desc:" + [lengthDesc.value, lengthDesc.writable, lengthDesc.enumerable, lengthDesc.configurable].join(","));
console.log("named name desc:" + [nameDesc.value, nameDesc.writable, nameDesc.enumerable, nameDesc.configurable].join(","));
console.log("named hasOwn length:" + Object.hasOwn(namedAdd, "length"));
console.log("named hasOwn name:" + Object.hasOwn(namedAdd, "name"));
console.log("named proto hasOwn length:" + Object.prototype.hasOwnProperty.call(namedAdd, "length"));
console.log("named proto hasOwn name:" + Object.prototype.hasOwnProperty.call(namedAdd, "name"));
console.log("named value match:" + (namedAdd.length === lengthDesc.value) + "," + (namedAdd.name === nameDesc.value));

const dynamicAdd = Function("left", "right", "return left + right;");
const dynamicLengthDesc = Object.getOwnPropertyDescriptor(dynamicAdd, "length");
const dynamicNameDesc = Object.getOwnPropertyDescriptor(dynamicAdd, "name");

console.log("dynamic length desc:" + [dynamicLengthDesc.value, dynamicLengthDesc.writable, dynamicLengthDesc.enumerable, dynamicLengthDesc.configurable].join(","));
console.log("dynamic name desc:" + [dynamicNameDesc.value, dynamicNameDesc.writable, dynamicNameDesc.enumerable, dynamicNameDesc.configurable].join(","));
console.log("dynamic hasOwn length:" + Object.hasOwn(dynamicAdd, "length"));
console.log("dynamic hasOwn name:" + Object.hasOwn(dynamicAdd, "name"));
console.log("dynamic value match:" + (dynamicAdd.length === dynamicLengthDesc.value) + "," + (dynamicAdd.name === dynamicNameDesc.value));
