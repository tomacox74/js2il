"use strict";\r\n\r\n// Nested destructuring with defaults
const obj = { a: { b: 5 } };
const { a: { b, c = 99 } } = obj;

console.log('b=', b);
console.log('c=', c);
