"use strict";

var plain = {};
var tagged = {};
tagged[Symbol.toStringTag] = "CustomThing";

var arr = [];
arr[Symbol.toStringTag] = "Bag";

console.log(Object.prototype.toString.call(plain));
console.log(Object.prototype.toString.call(tagged));
console.log(Object.prototype.toString.call(arr));
