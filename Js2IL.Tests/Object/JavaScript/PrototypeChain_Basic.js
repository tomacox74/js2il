"use strict";

// Basic prototype chaining via __proto__/Object.getPrototypeOf/setPrototypeOf

const proto = { a: 1 };
const o = {};
o.__proto__ = proto;
console.log(o.a);
console.log(Object.getPrototypeOf(o) === proto);
console.log(o.__proto__ === proto);

const p2 = { b: 2 };
const o2 = {};
console.log(Object.setPrototypeOf(o2, p2) === o2);
console.log(o2.b);

const o3 = {};
Object.setPrototypeOf(o3, null);
console.log(Object.getPrototypeOf(o3) === null);
console.log(o3.__proto__ === null);
console.log("b" in o3);