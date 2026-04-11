"use strict";

var o = {};
Object.defineProperties(o, {
  a: { value: 1, enumerable: true, writable: true, configurable: true },
  hidden: { value: 2, enumerable: false, writable: true, configurable: true }
});

console.log(o.a);
var keys = [];
for (var k in o) {
  keys.push(k);
}
console.log(keys.join(","));
console.log(Object.getOwnPropertyDescriptor(o, "hidden").enumerable === false);
