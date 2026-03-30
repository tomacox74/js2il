"use strict";

// RegExp.prototype.toString() should return /source/flags format

var r1 = /abc/gim;
console.log(r1.toString());

var r2 = /test/;
console.log(r2.toString());

var r3 = RegExp('hello', 'i');
console.log(r3.toString());
