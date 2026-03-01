"use strict";
// Risk area: integer arithmetic + - * / % **

console.log(3 + 4);    // 7
console.log(10 - 3);   // 7
console.log(3 * 4);    // 12
console.log(15 / 3);   // 5
console.log(17 % 5);   // 2
console.log(2 ** 8);   // 256

// Compound expressions
console.log((3 + 4) * (10 - 3)); // 49
console.log(100 / (2 * 5));       // 10

// Operator precedence
console.log(2 + 3 * 4);   // 14
console.log((2 + 3) * 4); // 20

// Negative numbers
console.log(-7 + 3);   // -4
console.log(-5 * -3);  // 15
console.log(-10 % 3);  // -1

// Bitwise as integer floor
console.log(7 | 0);    // 7
console.log(7.9 | 0);  // 7

// Unary minus / plus
let v = 5;
console.log(-v);  // -5
console.log(+v);  // 5

// Increment / decrement
let c = 10;
c++;
console.log(c); // 11
c--;
c--;
console.log(c); // 9
