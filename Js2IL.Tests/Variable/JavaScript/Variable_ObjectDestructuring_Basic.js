"use strict";\r\n\r\n// Test basic object destructuring assignment
const obj = { x: 10, y: 20, name: 'test' };
const { x, y, name } = obj;

console.log('x=', x);
console.log('y=', y);
console.log('name=', name);

// Verify values are independent copies
const sum = x + y;
console.log('sum=', sum);
