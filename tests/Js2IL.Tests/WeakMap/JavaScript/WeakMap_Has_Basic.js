"use strict";

var wm = new WeakMap();
var obj1 = {};
var obj2 = {};
wm.set(obj1, "value1");
console.log(wm.has(obj1));
console.log(wm.has(obj2));
