"use strict";

var parts = "a,b,c".split(/,/);
console.log(parts.length);
console.log(parts[0]);
console.log(parts[1]);
console.log(parts[2]);

var limited = "a,b,c".split(/,/, 2);
console.log(limited.length);
console.log(limited[0]);
console.log(limited[1]);
