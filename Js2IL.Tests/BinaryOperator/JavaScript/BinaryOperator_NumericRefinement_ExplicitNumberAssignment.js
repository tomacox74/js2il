"use strict";

// Tests that after x = Number(x), subsequent uses of x do not call ToNumber again.
function numberAssignmentRefinement(x) {
    x = Number(x);
    const a = x >>> 5;
    const b = x & 31;
    return a + b;
}

console.log(numberAssignmentRefinement(100));  // 3 + 4 = 7
console.log(numberAssignmentRefinement("100")); // parses to 100, same result: 7
console.log(numberAssignmentRefinement(0));
