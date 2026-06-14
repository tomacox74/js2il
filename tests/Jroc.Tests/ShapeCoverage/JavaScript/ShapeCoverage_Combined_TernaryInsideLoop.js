"use strict";

// Combined shape: ternary inside loop feeding a loop-carried accumulator.
// Tests join materialization and back-edge slot materialization together.
// i=0: even -> x=0; i=1: odd -> x=2; i=2: even -> x=2; i=3: odd -> x=6; i=4: even -> x=4
// Expected: 14
let sum = 0;
for (let i = 0; i < 5; i++) {
  const x = (i % 2 === 0) ? i : i * 2;
  sum = sum + x;
}
console.log(sum); // 14
