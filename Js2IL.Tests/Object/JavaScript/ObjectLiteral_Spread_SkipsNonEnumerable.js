"use strict";

// Spread should only copy enumerable own properties.

const o = {};
Object.defineProperty(o, "hidden", { value: 42, enumerable: false });
Object.defineProperty(o, "visible", { value: 7, enumerable: true });

const c = { ...o };

console.log(c.hidden);
console.log(c.visible);
console.log(Object.getOwnPropertyNames(c).join(","));
