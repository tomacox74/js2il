"use strict";

const o = {};
Object.defineProperty(o, "a", { value: 1, enumerable: false });
Object.defineProperty(o, "b", { value: 2, enumerable: true });

const keys = [];
for (const k in o) {
  keys.push(k);
}

console.log(keys.join(","));
