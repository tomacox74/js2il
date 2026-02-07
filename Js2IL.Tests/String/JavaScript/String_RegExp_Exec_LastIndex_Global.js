"use strict";

var r = /a/g;
var s = "baaa";

console.log(r.lastIndex);

var m1 = r.exec(s);
console.log(m1[0]);
console.log(m1.index);
console.log(m1.input);
console.log(r.lastIndex);

var m2 = r.exec(s);
console.log(m2[0]);
console.log(m2.index);
console.log(r.lastIndex);
