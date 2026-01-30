"use strict";

// Object literal computed keys

const k = 'x';
console.log(({ [k]: 1 }).x);

const o = { ['a' + 1]: 2 };
console.log(o.a1);
