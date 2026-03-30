"use strict";

// Destructuring assignment expression
let x = 0;
let y = 0;
({ x, y } = { x: 7, y: 9 });

console.log('x=', x);
console.log('y=', y);
