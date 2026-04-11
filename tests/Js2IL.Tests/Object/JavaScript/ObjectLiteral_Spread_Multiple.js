"use strict";

// Multiple spreads: later spreads override earlier ones.

const a = { x: 1, shared: "a" };
const b = { y: 2, shared: "b" };
const c = { z: 3, shared: "c" };

const o = { ...a, ...b, ...c };
console.log(o.x);
console.log(o.y);
console.log(o.z);
console.log(o.shared);
