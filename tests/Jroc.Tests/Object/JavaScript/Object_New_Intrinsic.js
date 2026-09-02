"use strict";

const ordinary = new Object();
console.log(ordinary.toString());

const existing = { value: 42 };
console.log(new Object(existing) === existing);

const wrapped = new Object(7);
console.log(typeof wrapped, wrapped.valueOf());

let effects = 0;
new Object(null, effects++);
console.log(effects);
