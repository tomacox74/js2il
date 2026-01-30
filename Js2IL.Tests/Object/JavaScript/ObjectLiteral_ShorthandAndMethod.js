"use strict";

// Shorthand properties and method definitions

const a = 1;
console.log(({ a }).a);

const o = { x: 7, m() { return this.x; } };
console.log(o.m());
