"use strict";

var buf = [65, 66, 67, 68, 69];
var part = buf.slice(1, 4);

console.log(part.length);
console.log(String.fromCharCode.apply(String, part));
console.log(buf.length);
