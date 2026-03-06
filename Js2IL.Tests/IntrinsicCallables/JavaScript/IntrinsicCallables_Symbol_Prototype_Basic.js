"use strict";

const sym = Symbol("desc");

console.log(sym.description);
console.log(sym.toString());
console.log(sym.valueOf() === sym);
