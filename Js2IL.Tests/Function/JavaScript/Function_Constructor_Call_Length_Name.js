"use strict";

const add = Function("a", "b", "return a + b;");

console.log(add(2, 3));
console.log(add.length);
console.log(add.name);
