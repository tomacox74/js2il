"use strict";

// Loop-carried variable: conditionally updated in one branch inside loop.
// Variable must retain its value on iterations where the branch is not taken.
// Expected: 6
let acc = 0;
for (let i = 0; i < 5; i++) {
  if (i % 2 === 0) {
    acc = acc + i;
  }
}
console.log(acc); // 0+2+4 = 6
