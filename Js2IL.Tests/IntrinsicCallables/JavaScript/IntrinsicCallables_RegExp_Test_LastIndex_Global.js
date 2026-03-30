"use strict";

// RegExp.prototype.test should update lastIndex for global regexes
var r = /a/g;
var s = "aaa";

console.log(r.lastIndex); // 0

console.log(r.test(s)); // true
console.log(r.lastIndex); // 1

console.log(r.test(s)); // true
console.log(r.lastIndex); // 2

console.log(r.test(s)); // true
console.log(r.lastIndex); // 3

console.log(r.test(s)); // false
console.log(r.lastIndex); // 0
