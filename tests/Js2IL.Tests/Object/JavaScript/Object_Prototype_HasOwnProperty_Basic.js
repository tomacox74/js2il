"use strict";

// Object.prototype.hasOwnProperty ( V )

var proto = { p: 123 };
var o = Object.create(proto);
o.own = 456;

console.log('own=' + Object.prototype.hasOwnProperty.apply(o, ['own']));
console.log('p=' + Object.prototype.hasOwnProperty.apply(o, ['p']));
