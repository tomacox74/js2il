"use strict";

var proto = { p: 1 };
var o = Object.create(proto);
o.own = 2;

console.log(Object.hasOwn(o, "own"));
console.log(Object.hasOwn(o, "p"));
