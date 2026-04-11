"use strict";

// Nested object literals with spreads.

const inner = { x: 1, y: 2 };
const outer = { nested: { ...inner } };

console.log(outer.nested.x);
console.log(outer.nested.y);
