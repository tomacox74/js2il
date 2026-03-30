"use strict";

var o = {};
Object.defineProperty(o, "hidden", {
  value: 1,
  enumerable: false,
  writable: true,
  configurable: true
});
o.visible = 2;

console.log(Object.prototype.propertyIsEnumerable.call(o, "visible"));
console.log(Object.prototype.propertyIsEnumerable.call(o, "hidden"));
console.log(Object.prototype.toLocaleString.call(o) === Object.prototype.toString.call(o));
console.log(Object.prototype.valueOf.call(o) === o);
