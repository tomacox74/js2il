"use strict";

var r = /a*/g;
var s = "b";

console.log(r.lastIndex);

var m1 = r.exec(s);
console.log(m1[0].length);
console.log(r.lastIndex);

var m2 = r.exec(s);
console.log(m2[0].length);
console.log(r.lastIndex);

var m3 = r.exec(s);
console.log(m3 === null);
console.log(r.lastIndex);
