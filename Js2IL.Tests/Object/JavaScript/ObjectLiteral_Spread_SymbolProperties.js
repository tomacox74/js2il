"use strict";

// Symbol-keyed properties should be copied by object spread.

const s = Symbol("k");
const o = { [s]: 123, a: 1 };

const c = { ...o };

console.log(c[s]);
console.log(c.a);
