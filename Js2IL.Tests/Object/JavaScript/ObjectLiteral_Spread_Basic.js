"use strict";\r\n\r\n// Object literal spread properties

const o1 = { ...{ a: 1 }, b: 2 };
console.log(o1.a);
console.log(o1.b);

// Overwrite order: later members win
const o2 = { a: 1, ...{ a: 2 } };
console.log(o2.a);

const o3 = { ...{ a: 1 }, a: 2 };
console.log(o3.a);

// null/undefined spreads are ignored
const o4 = { ...null, ...undefined, a: 1 };
console.log(o4.a);
