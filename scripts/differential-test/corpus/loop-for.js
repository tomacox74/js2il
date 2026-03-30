"use strict";
// Risk area: for-loop back-edges and variable updates
// Note: uses var (not let) for outer accumulators – see known issue with
// compound-assignment += when the RHS is a for-let iteration variable.

// Simple accumulator
var sum = 0;
for (var i = 1; i <= 10; i++) {
    sum += i;
}
console.log(sum); // 55

// Loop with multiple variable updates (Fibonacci)
var a = 1, b = 1;
for (var n = 2; n < 9; n++) {
    var next = a + b;
    a = b;
    b = next;
}
console.log(b); // 34

// Nested loops
var count = 0;
for (var i2 = 0; i2 < 4; i2++) {
    for (var j = 0; j < 4; j++) {
        if (i2 !== j) count++;
    }
}
console.log(count); // 12

// Loop with break and continue
var total = 0;
for (var k = 0; k < 20; k++) {
    if (k === 15) break;
    if (k % 3 === 0) continue;
    total += k;
}
console.log(total); // 1+2+4+5+7+8+10+11+13+14 = 75

// Decrementing loop
var product = 1;
for (var m = 5; m >= 1; m--) {
    product *= m;
}
console.log(product); // 120
