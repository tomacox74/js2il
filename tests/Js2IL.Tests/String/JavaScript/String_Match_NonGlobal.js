"use strict";

var s = "  abc";
var m = s.match(/\s+(a)(b)c/);

console.log(m[0]);
console.log(m[1]);
console.log(m[2]);
console.log(m.index);
console.log(m.input);
