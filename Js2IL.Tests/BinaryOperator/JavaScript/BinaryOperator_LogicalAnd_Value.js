"use strict";\r\n\r\nconst a = 1;
const b = "world";
const c = a && b; // should pick b since a is truthy
console.log(c);
