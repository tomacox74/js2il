"use strict";

const o = Object.create(null);
console.log(Object.getPrototypeOf(o) === null);
console.log(o.toString === undefined);

Object.defineProperty(o, "x", { value: 1, enumerable: true });
console.log(o.x);

const d = Object.getOwnPropertyDescriptor(o, "x");
console.log(d.value);
console.log(d.enumerable);
