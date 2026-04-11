"use strict";

// Loop-carried variable: read and updated on every back-edge.
// The accumulator must not be stale after any back-edge store.
// Expected: 15
let acc = 0;
for (let i = 1; i <= 5; i++) {
  acc = acc + i;
}
console.log(acc); // 1+2+3+4+5 = 15
