"use strict";

// Repro for domino pattern:
//   var pushAll = Function.prototype.apply.bind(Array.prototype.push);

var pushAll = Function.prototype.apply.bind(Array.prototype.push);

var a = [];

pushAll(a, [1, 2, 3]);
console.log(a.length);
console.log(a[0]);
console.log(a[2]);

var len = pushAll(a, [4, 5]);
console.log(len);
console.log(a.length);
console.log(a[4]);
