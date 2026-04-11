"use strict";

// RegExp.prototype.flags should return flags in alphabetical order

var r1 = /abc/gim;
console.log(r1.flags);

var r2 = /test/;
console.log(r2.flags);

var r3 = RegExp('hello', 'mi');
console.log(r3.flags);

var r4 = RegExp('world', 'g');
console.log(r4.flags);
