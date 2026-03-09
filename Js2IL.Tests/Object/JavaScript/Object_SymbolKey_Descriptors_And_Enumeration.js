"use strict";

var visible = Symbol("visible");
var hidden = Symbol("hidden");
var obj = {};

Object.defineProperty(obj, visible, {
  value: 1,
  enumerable: true,
  writable: false,
  configurable: true
});

Object.defineProperty(obj, hidden, {
  value: 2,
  enumerable: false,
  writable: true,
  configurable: false
});

var descriptor = Object.getOwnPropertyDescriptor(obj, visible);
var descriptors = Object.getOwnPropertyDescriptors(obj);
var symbols = Object.getOwnPropertySymbols(obj);
var keys = Object.keys(obj);
var forIn = [];
for (var key in obj) {
  forIn.push(key);
}

console.log(descriptor.value === 1);
console.log(descriptor.enumerable === true);
console.log(descriptor.writable === false);
console.log(descriptors[visible].value === 1);
console.log(descriptors[hidden].configurable === false);
console.log((symbols[0] === visible) + "," + (symbols[1] === hidden) + "," + symbols.length);
console.log(keys.length);
console.log(forIn.length);
