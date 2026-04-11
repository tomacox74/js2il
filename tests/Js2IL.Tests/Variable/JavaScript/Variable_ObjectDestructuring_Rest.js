"use strict";

// Object rest in destructuring
const obj = { a: 1, b: 2, c: 3 };
const { a, ...rest } = obj;

console.log('a=', a);
console.log('rest.b=', rest.b);
console.log('rest.c=', rest.c);
