"use strict";

var r = /a/y;
var s = "baaa";

r.lastIndex = 1;
console.log(r.test(s));
console.log(r.lastIndex);
console.log(r.test(s));
console.log(r.lastIndex);
console.log(r.test(s));
console.log(r.lastIndex);
console.log(r.test(s));
console.log(r.lastIndex);

r.lastIndex = 0;
console.log(r.test(s));
console.log(r.lastIndex);

r.lastIndex = Infinity;
console.log(r.test(s));
console.log(r.lastIndex);
