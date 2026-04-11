"use strict";

var s1 = Symbol("a");
var s2 = Symbol("b");
var o = {};
o[s1] = 1;
Object.defineProperty(o, s2, {
  value: 2,
  enumerable: false,
  writable: true,
  configurable: true
});

var syms = Object.getOwnPropertySymbols(o);
var hasS1 = false;
var hasS2 = false;
for (var i = 0; i < syms.length; i++) {
  if (syms[i] === s1) hasS1 = true;
  if (syms[i] === s2) hasS2 = true;
}

console.log("len=" + syms.length);
console.log("has_s1=" + hasS1);
console.log("has_s2=" + hasS2);
