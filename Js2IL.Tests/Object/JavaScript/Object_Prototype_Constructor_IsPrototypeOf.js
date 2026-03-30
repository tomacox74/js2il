"use strict";

var proto = {};
var o = Object.create(proto);

console.log(Object.prototype.constructor === Object);
console.log(Object.prototype.isPrototypeOf.call(proto, o));
console.log(Object.prototype.isPrototypeOf.call(o, proto));
