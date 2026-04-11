"use strict";

var b0 = new Boolean();
var b1 = new Boolean(0);
var b2 = new Boolean("");
var b3 = new Boolean("x");
var b4 = new Boolean(null);

console.log(typeof b0);
console.log(b0.valueOf());
console.log(b1.valueOf());
console.log(b2.valueOf());
console.log(b3.valueOf());
console.log(b4.valueOf());
console.log(b0.toString());
console.log(Boolean.prototype.constructor === Boolean);
console.log(Boolean.prototype.toString.call(false));
console.log(Boolean.prototype.valueOf.call(true));
