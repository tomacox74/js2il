"use strict";

var wm = new WeakMap();
var obj = {};
wm.set(obj, "value1");
console.log(wm.delete(obj));
console.log(wm.has(obj));
var obj2 = {};
console.log(wm.delete(obj2));
