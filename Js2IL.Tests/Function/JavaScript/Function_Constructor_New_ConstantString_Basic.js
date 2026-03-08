"use strict";

const f = new Function("return 1;");

console.log(f());
console.log(f.length);
console.log(f.name);
