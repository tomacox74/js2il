"use strict";

// Tests that when a parameter is used in multiple numeric bitwise operations,
// the ToNumber conversion is only performed once (CSE / numeric refinement).
function bitwiseParamTwice(x) {
    const a = x >>> 5;
    const b = x & 31;
    return a + b;
}

console.log(bitwiseParamTwice(100));  // 100 >>> 5 = 3, 100 & 31 = 4, 3 + 4 = 7
console.log(bitwiseParamTwice(0));
console.log(bitwiseParamTwice(255)); // 255 >>> 5 = 7, 255 & 31 = 31, 7 + 31 = 38
