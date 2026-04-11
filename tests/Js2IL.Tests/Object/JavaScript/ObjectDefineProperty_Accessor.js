"use strict";

let backing = 0;
const o = {};

Object.defineProperty(o, "x", {
  get: function () { console.log("get"); return backing; },
  set: function (v) { console.log("set:" + v); backing = v; },
  enumerable: true,
  configurable: true
});

console.log(o.x);
o.x = 42;
console.log(o.x);
