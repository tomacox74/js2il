"use strict";
// Risk area: while-loop back-edges with conditional variable updates

// Collatz sequence from 6
let n = 6;
let steps = 0;
while (n !== 1) {
    if (n % 2 === 0) {
        n = n / 2;
    } else {
        n = 3 * n + 1;
    }
    steps++;
}
console.log(steps); // 8

// do-while: executes at least once
let x = 1;
do {
    x *= 2;
} while (x < 64);
console.log(x); // 64

// while with break
let y = 0;
while (true) {
    y++;
    if (y >= 7) break;
}
console.log(y); // 7

// while accumulating only even values
let evens = 0;
let i = 0;
while (i < 20) {
    if (i % 2 === 0) evens += i;
    i++;
}
console.log(evens); // 0+2+4+6+8+10+12+14+16+18 = 90

// Nested while
let outer = 0, inner = 0;
let oi = 0;
while (oi < 3) {
    let ij = 0;
    while (ij < 3) {
        inner++;
        ij++;
    }
    outer++;
    oi++;
}
console.log(outer); // 3
console.log(inner); // 9
