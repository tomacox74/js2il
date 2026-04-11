"use strict";

var o = { a: 1 };
Object.defineProperty(o, "hidden", {
  value: 2,
  enumerable: false,
  writable: false,
  configurable: false
});

var d = Object.getOwnPropertyDescriptors(o);

console.log(d.a.value === 1);
console.log(d.a.enumerable === true);
console.log(d.hidden.value === 2);
console.log(d.hidden.enumerable === false);
console.log(d.hidden.writable === false);
console.log(d.hidden.configurable === false);
