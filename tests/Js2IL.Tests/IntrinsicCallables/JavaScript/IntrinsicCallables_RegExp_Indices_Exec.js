"use strict";

var first = /(a)?b/d.exec("b");
console.log(first !== null);
console.log(first.indices.length);
console.log(first.indices[0][0]);
console.log(first.indices[0][1]);
console.log(first.indices[1] === null);

var second = /a(d)?/d.exec("bad");
console.log(second.indices[0][0]);
console.log(second.indices[0][1]);
console.log(second.indices[1][0]);
console.log(second.indices[1][1]);
