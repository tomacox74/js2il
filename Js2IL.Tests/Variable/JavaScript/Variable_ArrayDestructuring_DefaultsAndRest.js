"use strict";\r\n\r\n// Array destructuring with defaults and rest
const arr = [10];
const [a = 1, b = 2, ...rest] = arr;

console.log('a=', a);
console.log('b=', b);
console.log('rest.length=', rest.length);
console.log('rest0=', rest[0]);

// Ensure rest element loop executes (rest has elements)
const arr2 = [1, 2, 3, 4];
const [x, y, ...rest2] = arr2;
console.log('x=', x);
console.log('y=', y);
console.log('rest2.length=', rest2.length);
console.log('rest20=', rest2[0]);
console.log('rest21=', rest2[1]);
