"use strict";

const fn = async function* sample(a, b) { };
const fnProto = Object.getPrototypeOf(fn);
const ctor = fnProto.constructor;
const lengthDesc = Object.getOwnPropertyDescriptor(ctor, "length");

console.log(typeof ctor);
console.log(ctor.length);
console.log([lengthDesc.value, lengthDesc.writable, lengthDesc.enumerable, lengthDesc.configurable].join(","));
console.log(fnProto === ctor.prototype);
