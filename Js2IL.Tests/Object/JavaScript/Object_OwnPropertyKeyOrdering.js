"use strict";

var symA = Symbol("a");
var symB = Symbol("b");
var o = {};

o.beta = "b";
o[2] = "two";
Object.defineProperty(o, "hidden", {
  value: "hidden",
  enumerable: false,
  configurable: true,
  writable: true
});
o.alpha = "a";
o[1] = "one";
o[symA] = "symA";
Object.defineProperty(o, symB, {
  value: "symB",
  enumerable: false,
  configurable: true,
  writable: true
});

var names = Object.getOwnPropertyNames(o);
var keys = Object.keys(o);
var values = Object.values(o);
var entries = Object.entries(o);
var forIn = [];
for (var k in o) {
  forIn.push(k);
}

var entryPairs = [];
for (var i = 0; i < entries.length; i++) {
  entryPairs.push(entries[i][0] + ":" + entries[i][1]);
}

var syms = Object.getOwnPropertySymbols(o);
console.log("names=" + names.join(","));
console.log("keys=" + keys.join(","));
console.log("values=" + values.join(","));
console.log("entries=" + entryPairs.join(","));
console.log("forin=" + forIn.join(","));
console.log("symbols=" + (syms[0] === symA) + "," + (syms[1] === symB) + "," + syms.length);
