"use strict";

var wm = new WeakMap();
var obj = {};
wm.set(obj, "value1");
console.log(wm.get(obj));
