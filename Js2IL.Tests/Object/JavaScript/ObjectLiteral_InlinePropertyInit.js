"use strict";

// Object literal used inline (non-materialized) to exercise inlined literal emission.
console.log(({ a: 1, b: 2 }).a);
console.log(({ a: 1, b: 2 })["b"]);

function getX() {
  return ({ x: 10, y: 20 }).x;
}

console.log(getX());
